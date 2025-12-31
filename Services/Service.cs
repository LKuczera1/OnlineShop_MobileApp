using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public class Service
    {
        protected HttpClient httpClient;

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

        public Service(HttpClient client)
        {
            httpClient = client;
        }

        protected void SetCancelationToken()
        {
            cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(connectionTimeout));
        }

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
    }
}
