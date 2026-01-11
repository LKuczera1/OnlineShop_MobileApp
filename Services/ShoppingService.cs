using OnlineShop_MobileApp.Models.DTOs;
using OnlineShop_MobileApp.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public class ShoppingService : Service, IShoppingService
    {

        private const string CartItemsEndpoint = "/api/ShoppingCartItems";
        private const string InsertItemToCartEndpoint = "/api/ShoppingCartItems";
        public ShoppingService(HttpClient client, ITokenStore tokenStore)
            : base(client, tokenStore)
        {
        }

        public async Task<List<CartItemDto>> GetCartItems()
        {
            HttpResponseMessage response = await AuthorizedGetAsync(CartItemsEndpoint);

            if (!response.IsSuccessStatusCode)
                return new List<CartItemDto>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<CartItemDto>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            ) ?? new List<CartItemDto>();
        }

        public async Task<bool> InsertItemToCart(int productId, double Quantity = 1)
        {
            string endpoint = InsertItemToCartEndpoint + $"/{productId}/{Quantity}";

            HttpResponseMessage response = await AuthorizedSendAsync(endpoint);

            if (response.IsSuccessStatusCode) return true;
            else return false;
        }
    }
}
