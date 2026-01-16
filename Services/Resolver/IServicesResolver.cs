using OnlineShop_MobileApp.Models;

namespace OnlineShop_MobileApp.Services.Resolver
{
    public interface IServicesResolver
    {
        public Task<Product?> ResolveForProduct(int productId);
        public Task<byte[]?> ResolveForProductThumbnail(int productId);
        public Task<bool> ResolveInsertItemIntoCart(int productId);
    }
}
