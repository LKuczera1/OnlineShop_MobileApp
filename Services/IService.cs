using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public class Service
    {
        protected readonly HttpClient _httpClient;

        public Service(HttpClient client)
        {
            _httpClient = client;
        }
    }
}
