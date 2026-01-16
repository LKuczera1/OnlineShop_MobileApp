using OnlineShop_MobileApp.Services.Authentication;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OnlineShop_MobileApp.Services
{
    public class Service
    {
        protected HttpClient httpClient;
        private readonly ITokenStore _tokenStore;

        protected CancellationTokenSource cts;
        protected int connectionTimeout = 3; //Cancelation token timeout in seconds

        public Service(HttpClient client, ITokenStore tokenStore)
        {
            httpClient = client;
            _tokenStore = tokenStore;
        }

        /// <summary>
        /// This method refreshes the cancelation token with specified time. It is recommended to call this method before each use of the CTS.
        /// </summary>
        protected void SetCancelationToken()
        {
            cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(connectionTimeout));
        }

        /// <summary>
        /// Checks if JWT token is still valid.
        /// </summary>
        /// <returns></returns>
        protected async Task<bool> CheckJWTTokenStatus()
        {
            try
            {
                return await _tokenStore.EnsureTokenActivity();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Refreshes Cancelation Token and fetches new JWT token
        /// </summary>
        /// <returns></returns>
        protected async Task<AuthSession> GetJWTToken()
        {
            SetCancelationToken();
            return await _tokenStore.GetAsync();
        }

        /// <summary>
        /// Method used to send data to APIs endpoint without authorization. url - API's endpoint, messageContent - Content of message,
        /// httpMethod - target CRUD operation (GET / POST / PUT / DELETE etc.), it's post by default, ignoreStatusCode - if it's true then
        /// method returns HttpResponseMessage regardles status code. Ignore status code is false by default, so this method may throw 
        /// excepction if response code is different than OK - 200.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="messageContent"></param>
        /// <param name="httpMethod"></param>
        /// <param name="HttpCmpOptions"></param>
        /// <param name="ignoreStatusCode"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> SendAsync(string url,
            JsonContent? messageContent = null,
            HttpMethod? httpMethod = null,
            HttpCompletionOption HttpCmpOptions = HttpCompletionOption.ResponseHeadersRead,
            bool ignoreStatusCode = false)
        {
            SetCancelationToken();


            if (httpMethod is null) httpMethod = HttpMethod.Post;

            HttpRequestMessage request = new HttpRequestMessage(httpMethod, url);
            if (messageContent != null) { request.Content = messageContent; }

            var response = await httpClient.SendAsync(request, HttpCmpOptions, cts.Token);

            if (ignoreStatusCode) return response;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw;
            }

            return response;
        }

        /// <summary>
        /// Method used to fetch data from APIs endpoint without authorization. url - API's endpointm
        /// httpMethod - target CRUD operation (GET / POST / PUT / DELETE etc.), it's GET by default, ignoreStatusCode - if it's true then
        /// method returns HttpResponseMessage regardles status code. Ignore status code is false by default, so this method may throw 
        /// excepction if response code is different than OK - 200.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <param name="HttpCmpOptions"></param>
        /// <param name="ignoreStatusCode"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> GetAsync(string url,
            HttpMethod? httpMethod = null,
            HttpCompletionOption HttpCmpOptions = HttpCompletionOption.ResponseHeadersRead,
            bool ignoreStatusCode = false)
        {
            SetCancelationToken();


            if (httpMethod is null) httpMethod = HttpMethod.Get;

            HttpRequestMessage request = new HttpRequestMessage(httpMethod, url);

            var response = await httpClient.SendAsync(request, HttpCmpOptions, cts.Token);

            if (ignoreStatusCode) return response;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw;
            }

            return response;
        }

        /// <summary>
        /// Method used to send data to APIs endpoint with authorization. url - API's endpoint, messageContent - Content of message,
        /// httpMethod - target CRUD operation (GET / POST / PUT / DELETE etc.), it's post by default, ignoreStatusCode - if it's true then
        /// method returns HttpResponseMessage regardles status code. Ignore status code is false by default, so this method may throw 
        /// excepction if response code is different than OK - 200.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="messageContent"></param>
        /// <param name="httpMethod"></param>
        /// <param name="HttpCmpOptions"></param>
        /// <param name="ignoreStatusCode"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> AuthorizedSendAsync(string url,
            JsonContent? messageContent = null,
            HttpMethod? httpMethod = null,
            HttpCompletionOption HttpCmpOptions = HttpCompletionOption.ResponseHeadersRead,
            bool ignoreStatusCode = false)
        {
            //--- JWT logic ---

            string? JWTtoken = null;

            try
            {
                if (await CheckJWTTokenStatus())
                {
                    var session = await GetJWTToken();
                    if (session != null && session.AccessToken != null) JWTtoken = session.AccessToken;
                    else throw new Exception("JWT token error");
                }
                else
                {
                    SetCancelationToken();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while fetchin JWT token.");
            }

            //----------------

            if (httpMethod is null) httpMethod = HttpMethod.Post;

            HttpRequestMessage request = new HttpRequestMessage(httpMethod, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWTtoken);
            if (messageContent != null) { request.Content = messageContent; }

            var response = await httpClient.SendAsync(request, HttpCmpOptions, cts.Token);

            if (ignoreStatusCode) return response;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw;
            }

            return response;
        }

        /// <summary>
        /// Method used to fetch data from APIs endpoint with authorization. url - API's endpointm
        /// httpMethod - target CRUD operation (GET / POST / PUT / DELETE etc.), it's GET by default, ignoreStatusCode - if it's true then
        /// method returns HttpResponseMessage regardles status code. Ignore status code is false by default, so this method may throw 
        /// excepction if response code is different than OK - 200.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <param name="HttpCmpOptions"></param>
        /// <param name="ignoreStatusCode"></param>
        /// <returns></returns>

        protected async Task<HttpResponseMessage> AuthorizedGetAsync(string url,
            HttpMethod? httpMethod = null,
            HttpCompletionOption HttpCmpOptions = HttpCompletionOption.ResponseHeadersRead,
            bool ignoreStatusCode = false)
        {
            //--- JWT logic ---

            string? JWTtoken = null;

            try
            {
                if (await CheckJWTTokenStatus())
                {
                    var session = await GetJWTToken();
                    if (session != null && session.AccessToken != null) JWTtoken = session.AccessToken;
                    else throw new Exception("JWT token error");
                }
                else
                {
                    SetCancelationToken();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while fetchin JWT token.");
            }

            //----------------

            if (httpMethod is null) httpMethod = HttpMethod.Get;

            HttpRequestMessage request = new HttpRequestMessage(httpMethod, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", JWTtoken);

            var response = await httpClient.SendAsync(request, HttpCmpOptions, cts.Token);

            if (ignoreStatusCode) return response;

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw;
            }

            return response;
        }

        /// <summary>
        /// Return true if user is LoggedIn - Token is valid, and user is logged in.
        /// </summary>
        /// <returns></returns>
        public bool isUserLoggedIn()
        {
            return _tokenStore.IsUserLoggedIn;
        }
    }
}
