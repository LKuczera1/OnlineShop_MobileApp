using OnlineShop_MobileApp.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public interface IShoppingService
    {
        Task<List<CartItemDto>> GetCartItems();

        public bool isUserLoggedIn();

        public Task<bool> InsertItemToCart(int productId, double Quantity = 1);
    }
}
