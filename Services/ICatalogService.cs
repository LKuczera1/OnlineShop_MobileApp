using OnlineShop_MobileApp.Models;

namespace OnlineShop_MobileApp.Services
{
    public interface ICatalogService
    {
        public Task<List<Product>> GetProducts(int page);
        public Task<int> GetNumberOfPages();
        public Task<byte[]?> LoadProductThumbnail(int id);
        public Task<byte[]?> LoadProductPicture(int id);
        Task<Product?> GetProduct(int productId);
        public bool isUserLoggedIn();
    }
}
