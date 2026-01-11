using OnlineShop_MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services.Resolver
{
    public interface IServicesResolver
    {
        public Task<Product?> ResolveForProduct(int productId);
        public Task<byte[]?> ResolveForProductThumbnail(int productId);
        public Task<bool> ResolveInsertItemIntoCart(int productId);
    }
}
