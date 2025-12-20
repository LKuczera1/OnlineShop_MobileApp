using OnlineShop_MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public interface ICatalogService
    {
        public Task<List<Product>> GetProducts(int page, CancellationToken ct = default);
    }
}
