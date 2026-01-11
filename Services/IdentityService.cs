using OnlineShop_MobileApp.Models.DTOs;
using OnlineShop_MobileApp.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public class IdentityService: Service, IIdentityService
    {

        private readonly ITokenStore _tokenStore;

        public string loginendpoint = "/api/Accounts/login";

        public IdentityService(HttpClient client, IHttpClientFactory httpClientFactory, ITokenStore tokenStore) : base(client, tokenStore)
        {
            _tokenStore = tokenStore;
        }

        public async Task<bool> LoginAsync(string login, string password)
        {
            SetCancelationToken();

            var temp = new
            {
                userName = login,
                password = password,
            };

            JsonContent loginJson = JsonContent.Create(temp);

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

                        await _tokenStore.SetAuthSession(session, login);

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            

            return false;
        }
    }
}
