using OnlineShop_MobileApp.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public class Service
    {
        protected HttpClient httpClient;
        private readonly ITokenStore _tokenStore;

        protected CancellationTokenSource cts;
        protected int connectionTimeout = 3; //Cancelation token timeout in seconds


        //Custom exception to simplify possible scenarios...
        public class ConnectionErrorException : Exception
        {
            public enum ConnectionErrorType
            {
                CouldNotConnectToService = 0,
                ResponseTreatedAsError = 1,
            }

            public ConnectionErrorType _errorType { get; }

            public ConnectionErrorException(ConnectionErrorType errorType, string message = null) : base(message)
            {
                _errorType = errorType;
            }
            public ConnectionErrorException(ConnectionErrorType errorType, Exception e) :base(e.Message, e)
            {
                //Wraps received exception to my custom exception
                _errorType = errorType;
            }
        }

        //Zrobic uniwersalne metody zajmujace sie logika polaczen z rest api, tak aby dalo sie stworzyc funkcje
        //w glownych services z jak najmniejsza liniakodu

        public Service(HttpClient client, ITokenStore tokenStore)
        {
            httpClient = client;
            _tokenStore = tokenStore;
        }

        protected void SetCancelationToken() //Refresh Cancellation token
        {
            cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(connectionTimeout));
        }

        protected async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead,
        CancellationToken ct = default)
        {
            // 1) Dodaj token jeśli jest (jak swagger)
            var session = await _tokenStore.GetAsync();
            if (session is not null)
            {
                // bufor, żeby nie wysyłać z tokenem który zaraz padnie
                if (session.ExpiresAtUtc <= DateTimeOffset.UtcNow.AddSeconds(30))
                {
                    await _tokenStore.ClearAsync();
                    // Możesz tu rzucić wyjątek typu SessionExpiredException
                    // throw new SessionExpiredException();
                }
                else
                {
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", session.AccessToken);
                }
            }

            // 2) Wysyłka
            var response = await httpClient.SendAsync(request, completionOption, ct);

            // 3) Jeśli backend mówi 401 → czyścimy token
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _tokenStore.ClearAsync();
            }

            return response;
        }

        //To refactor

        /// <summary>
        /// Fetches response from REST API
        /// </summary>
        /// <param name="url"></param>
        /// <param name="HttpCmpOptions"></param>
        /// <returns></returns>
        /// <exception cref="ConnectionErrorException"></exception>
        protected async Task<HttpResponseMessage> GetAsync(string url,
            HttpCompletionOption HttpCmpOptions = HttpCompletionOption.ResponseHeadersRead)
        {
            SetCancelationToken();

            try
            {
                return await httpClient.GetAsync(
                    url,
                    HttpCmpOptions,
                    cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //implement logid where service decides is it connection error or reqest has been denied
                throw new ConnectionErrorException(ConnectionErrorException.ConnectionErrorType.CouldNotConnectToService,ex);
            }
        }

        //To refactor
        protected Task<HttpResponseMessage> PostAsync(
        string url,
        HttpContent content,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead,
        CancellationToken ct = default)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            return SendAsync(req, completionOption, ct);
        }

        //Note: Task<HttpResponseMessage> RefreshJWTToken()
        //This is simplest but not best solution to refresh token.
        //The best solution that provides scalability and protects from call lopps is:
        //1. Divide "Service" class to "ServiceBase" and "AuthorizedService: ServiceBase"
        //   Service base implements request that doesn't require JWT token and AuthorizedService
        //   implements methods using JWT token. But there is another issue: Using this method requires
        //   to implement 2 version of identityService: "AccountService: AuthorizedService", "IdentityService: ServiceBase"
        //   AccountService - Allows user to perform operation on account that requires JWT example: change password
        //   IdentityService - Implements only Login and Register operations.
        //
        //2. There was second good solution but i forgot it...
        //
        //   There is third option: Add new endpoint to API: /login/refresh - to refresh endpoint (requires only jwt token (+ username?))
        //   If token is about to expire we can just get jwt token from SecuretokenStore, send it - API veryfies it, and returns new token
        //   that is send to SecureTokenStore. Maybe it's not so bad option at all.
        //
        //   We can simply implement method "GetJWT" that checks if token is still active, if not token is beeing refreshed
        //   and user is still logged in. <- W sumie jedyne sensowne wyjscie w obecnej sytuacji. Zrobic backup tego brancha zeby miec potem
        //   te notatki...
        protected Task<HttpResponseMessage> RefreshJWTToken()
        {
            return null;
        }
    }
}
