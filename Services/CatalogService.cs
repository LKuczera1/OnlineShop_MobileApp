using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineShop_MobileApp.Services
{
    public class CatalogService: Service, ICatalogService
    {
        public CatalogService(HttpClient client) : base(null)
        {
            
            HttpClient test = client;
        }
    }
}
