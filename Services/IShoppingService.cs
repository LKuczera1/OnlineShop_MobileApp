using OnlineShop_MobileApp.Models.DTOs;
using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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
