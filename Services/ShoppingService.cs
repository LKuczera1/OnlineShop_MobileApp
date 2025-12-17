using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public class ShoppingService: Service
    {
        public ShoppingService(HttpClient client) : base(client)
        {
            
        }
    }
}
