using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public class CatalogService: Service, ICatalogService
    {
        public CatalogService(HttpClient client) : base(client)
        {
            HttpClient test = client;
            int x = 100;
            int y = 0;
            int z = x / y;
        }
    }
}
