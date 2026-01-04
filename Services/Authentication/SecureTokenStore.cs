using Microsoft.Maui.Storage;
using OnlineShop_MobileApp.Models.DTOs;
using OnlineShop_MobileApp.Services.Authentication;

public class SecureTokenStore : ITokenStore
{
    private const string TokenKey = "auth.access_token";
    private const string ExpKey = "auth.expires_at_utc";

    private CredentialStoreDto? _credentialStore;

    public CredentialStoreDto? CredentialStore
    { get { return _credentialStore; } }

    private readonly SemaphoreSlim _gate = new(1, 1);
    private AuthSession? _cache;

    //Gdy uzytkownik zostanie zalogowany, tworzony jest nowy obiekt CredentialStoreDto
    //Z danymi logowania. Za kazdym razem gdy wysylany jest request sprawdzamy czy
    //jwt jest wciaz valid, jezeli nie, to pobieramy stad credentialStore - dane logowania
    //i w klasie bazowej Service jest metoda RefreshJWT... blah blah blah...
    //Cholernie bezpieczne jezeli chodzi o bezpieczenstwo danych uzytkownika... No ale coz...
    //jest to projekt na "HumanComputerInteraction", gdzie liczy sie "wyglad" aplikacji, a nie na
    //"UserDataProtection". Moze kiedys zostanie to naprawione. +Nie chce mi sie dodawac specjalnych
    //Endpointow do api...

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

    public async Task SetAsync(AuthSession session)
    {
        await _gate.WaitAsync();
        try
        {
            _cache = session;
            await SecureStorage.SetAsync(TokenKey, session.AccessToken);
            await SecureStorage.SetAsync(ExpKey, session.ExpiresAtUtc.ToString("o"));
        }
        finally { _gate.Release(); }
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
        finally { _gate.Release(); }
    }

    public async Task<bool> IsTokenStillActive(int timeReserve = 30) //30 seconds before JWT token expires
    {
        var session = await GetAsync();
        if (session is null) return false;

        return session.ExpiresAtUtc > DateTimeOffset.UtcNow.AddSeconds(timeReserve);
    }
}