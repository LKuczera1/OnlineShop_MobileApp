using OnlineShop_MobileApp.Models.DTOs;
using OnlineShop_MobileApp.Services.Authentication;
using System.Net;
using System.Net.Http.Json;

namespace OnlineShop_MobileApp.Services
{
    public class IdentityService : Service, IIdentityService
    {

        private readonly ITokenStore _tokenStore;

        public string loginendpoint = "/api/Accounts/login";
        public string getUserDataEndpoint = "/api/Accounts/";
        public string registerEndpoint = "/api/Accounts/register";

        public IdentityService(HttpClient client, IHttpClientFactory httpClientFactory, ITokenStore tokenStore) : base(client, tokenStore)
        {
            _tokenStore = tokenStore;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            SetCancelationToken();

            JsonContent loginJson = JsonContent.Create(loginRequest);

            try
            {

                var response = await SendAsync(loginendpoint, loginJson, HttpMethod.Post);

                if (response != null)
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch
                    {
                        //Unable to connect - throw an exception
                    }
                    //---------------------------------------------------

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                        AuthSession session = new AuthSession();
                        session.AccessToken = loginResponse.Token;
                        session.ExpiresAtUtc = loginResponse.ExpiresAt;

                        await _tokenStore.SetAuthSession(session, loginRequest.UserName);

                        return loginResponse;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public async Task<UserDataDto?> GetUserData(int userId)
        {
            SetCancelationToken();

            string endpoint = getUserDataEndpoint + userId.ToString();

            HttpResponseMessage response = await AuthorizedGetAsync(endpoint);

            if (response != null)
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch
                {
                    //Unable to connect - throw an exception
                }
                //---------------------------------------------------

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var userdata = await response.Content.ReadFromJsonAsync<UserDataDto>();

                    return userdata;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        public async Task<RegisterResponseDto> CreateNewAccount(RegisterRequestDto regRequest)
        {
            SetCancelationToken();

            JsonContent messageContent = JsonContent.Create(regRequest);
            RegisterResponseDto regResponse = new RegisterResponseDto();

            HttpResponseMessage response = await SendAsync(registerEndpoint, messageContent, HttpMethod.Post, ignoreStatusCode: true);

            if (response != null)
            {

                var responseMessage = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    regResponse.RequestAccepted(responseMessage);
                }
                else
                {
                    regResponse.RequestDenied(responseMessage);
                }
            }
            else
            {
                regResponse.RequestDenied("Unknown error");
            }

            return regResponse;
        }

        public void LogOut()
        {
            _tokenStore.ClearAsync();
        }
    }
}
