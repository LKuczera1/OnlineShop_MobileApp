using Microsoft.Maui.Storage;
using OnlineShop_MobileApp.Models.DTOs;
using OnlineShop_MobileApp.Services.Authentication;
using System.ComponentModel;
using System.Net.Http.Json;

public class SecureTokenStore : ITokenStore
{
    private const string TokenKey = "auth.access_token";
    private const string ExpKey = "auth.expires_at_utc";

    public event PropertyChangedEventHandler? PropertyChanged;


    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    private CredentialStoreDto? _credentialStore;

    private string? _username = null;

    private bool _isUserLoggedIn = false;


    public bool IsUserLoggedIn
    {
        get => _isUserLoggedIn;
        private set
        {
            if (_isUserLoggedIn == value) return;
            _isUserLoggedIn = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUserLoggedIn)));
        }
    }
    public string? Username
    {
        get { return _username; }
    }

    public CredentialStoreDto? CredentialStore
    { get { return _credentialStore; } }

    private readonly SemaphoreSlim _gate = new(1, 1);

    private AuthSession? _cache;

    private readonly HttpClient _httpClient;
    private readonly string _refreshEndpoint;

    private const int tokenTimeoutReverveTime = 300; //5 minutes before JWT token expires

    //Gdy uzytkownik zostanie zalogowany, tworzony jest nowy obiekt CredentialStoreDto
    //Z danymi logowania. Za kazdym razem gdy wysylany jest request sprawdzamy czy
    //jwt jest wciaz valid, jezeli nie, to pobieramy stad credentialStore - dane logowania
    //i w klasie bazowej Service jest metoda RefreshJWT... blah blah blah...
    //Cholernie bezpieczne jezeli chodzi o bezpieczenstwo danych uzytkownika... No ale coz...
    //jest to projekt na "HumanComputerInteraction", gdzie liczy sie "wyglad" aplikacji, a nie na
    //"UserDataProtection". Moze kiedys zostanie to naprawione. +Nie chce mi sie dodawac specjalnych
    //Endpointow do api...
    

    public SecureTokenStore(HttpClient httpClient, string refreshEndpoint)
    {
        _httpClient = httpClient;
        _refreshEndpoint = refreshEndpoint;
        _cache = new AuthSession();
    }

    public async Task<AuthSession?> GetAsync()
    {
        if (_cache is not null) return _cache;

        await _gate.WaitAsync();
        try
        {
            if (_cache is not null) return _cache;

            var token = await SecureStorage.GetAsync(TokenKey);
            var exp = await SecureStorage.GetAsync(ExpKey);

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(exp))
                return null;

            if (!DateTimeOffset.TryParse(exp, out var expiresAtUtc))
                return null;

            _cache = new AuthSession(token, expiresAtUtc);
            return _cache;
        }
        finally { _gate.Release(); }
    }

    public async Task SetAsync(AuthSession session, string? login = null)
    {
        await _gate.WaitAsync();
        try
        {
            if(login!=null)
            {
                _username = login;
            }

            _cache = session;
            await SecureStorage.SetAsync(TokenKey, session.AccessToken);
            await SecureStorage.SetAsync(ExpKey, session.ExpiresAtUtc.ToString());
        }
        finally { _gate.Release(); }

        IsUserLoggedIn = true;
    }

    public async Task ClearAsync()
    {
        await _gate.WaitAsync();
        try
        {
            _cache = null;
            SecureStorage.Remove(TokenKey);
            SecureStorage.Remove(ExpKey);
        }
        finally { 
            _gate.Release();
            IsUserLoggedIn = false;
        }
    }

    /// <summary>
    /// This method checks checks activity status for JWT token and refreshes it 5 minutes before expiration time.
    /// It's obligatory to use this method every time request requiring authentication is send.
    /// </summary>
    /// <returns><c>true</c> if user is logged in or <c>false</c> if user is not logged in</returns>
    /// <exception cref="TokenRefreshException"></exception>
    public async Task<bool> EnsureTokenActivity()
    {
        bool isTokenActive = await IsTokenStillActive();

        if(isTokenActive) return true;
        else if(!_isUserLoggedIn) return false;

        try
        {
            return await RefreshToken();
        }
        catch
        {
            throw;
        }
    }

    private async Task<bool> IsTokenStillActive(int timeReserve = tokenTimeoutReverveTime) //5 minutes before JWT token expires
    {
        var session = await GetAsync();
        if (session is null) return false;

        return session.ExpiresAtUtc > DateTimeOffset.UtcNow.AddSeconds(timeReserve);
    }
    private async Task<bool> RefreshToken(int timeReserve = tokenTimeoutReverveTime)
    {
        //No token = User not logged in
        if (string.IsNullOrWhiteSpace(_cache.AccessToken))
            return false;

        //token is still valid = User is logged in
        if (_cache.ExpiresAtUtc > DateTimeOffset.UtcNow.AddSeconds(timeReserve))
            return true;

        await _refreshLock.WaitAsync();

        try
        {
            //Double check = Token refreshing might been invoked by another service
            if (_cache.ExpiresAtUtc > DateTimeOffset.UtcNow.AddSeconds(30))
                return true;

            var request = new HttpRequestMessage(HttpMethod.Post, _refreshEndpoint);

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cache.AccessToken);

            if (string.IsNullOrEmpty(_username)) throw new TokenRefreshException("An error occured while refreshign JWT token.");

            request.Content = JsonContent.Create(new
            {
                UserName = _username
            });

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new TokenRefreshException("Failed to call refresh endpoint", ex);
            }

            //401/403 -> token is not valid = UserNotLoggedIn
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                ClearAsync();
                return false;
            }

            if (!response.IsSuccessStatusCode)
                throw new TokenRefreshException(
                    $"Refresh endpoint returned {(int)response.StatusCode}");
            
            AuthResponseDto dto;
            try
            {
                dto = await response.Content.ReadFromJsonAsync<AuthResponseDto>()
                      ?? throw new TokenRefreshException("Empty refresh response.");
            }
            catch (Exception ex)
            {
                throw new TokenRefreshException("Invalid refresh response payload.", ex);
            }


            AuthSession temp = new AuthSession();

            temp.AccessToken = dto.Token;

            if (!DateTimeOffset.TryParse(dto.ExpiresAt, out var parsedDate))
                throw new TokenRefreshException("An error occured while parsing token expiration time.");

            temp.ExpiresAtUtc = parsedDate;
            _username = dto.UserName;

            await SetAsync(temp);

            return true;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    public async Task SetAuthSession(AuthSession session, string login)
    {
        await SetAsync(session, login);
    }
}