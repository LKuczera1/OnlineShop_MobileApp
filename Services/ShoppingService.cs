using OnlineShop_MobileApp.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public class ShoppingService: Service
    {
        public ShoppingService(HttpClient client, ITokenStore tokenStore) : base(client, tokenStore)
        {
            
        }
    }
}
