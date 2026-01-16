using OnlineShop_MobileApp.Models.DTOs;
using Shopping.Models;
using System.Net.Http.Json;

namespace OnlineShop_MobileApp.Services
{
    public interface IShoppingService
    {
        Task<List<CartItemDto>> GetCartItems();

        public bool isUserLoggedIn();

        public Task<bool> InsertItemToCart(int productId, double Quantity = 1);

        public Task<bool> RemoveCartItem(int cartItemId);

        public Task<bool> PlaceOrder(JsonContent content);

        public Task<List<Order>?> GetOrders();
    }
}
