using OnlineShop_MobileApp.Models.DTOs;
using OnlineShop_MobileApp.Services.Authentication;
using Shopping.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace OnlineShop_MobileApp.Services
{
    public class ShoppingService : Service, IShoppingService
    {
        //ToDo: Move endpoints from class to Config.json
        private const string CartItemsEndpoint = "/api/ShoppingCartItems";
        private const string InsertItemToCartEndpoint = "/api/ShoppingCartItems";
        private const string RemoveCartItemEndpoint = "/api/ShoppingCartItems/";
        private const string PlaceOrderEndpoint = "/api/ShoppingCartItems/PlaceOrder";
        private const string GetOrdersEndpoint = "/api/Orders";

        public ShoppingService(HttpClient client, ITokenStore tokenStore)
            : base(client, tokenStore){}


        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

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

        public async Task<bool> RemoveCartItem(int cartItemId)
        {
            string endpoint = RemoveCartItemEndpoint + cartItemId.ToString();

            HttpResponseMessage response = await AuthorizedSendAsync(endpoint, null, HttpMethod.Delete);

            if (response.IsSuccessStatusCode) return true;
            else return false;
        }

        public async Task<bool> PlaceOrder(JsonContent content)
        {
            HttpResponseMessage response = await AuthorizedSendAsync(PlaceOrderEndpoint, content);

            if (response.IsSuccessStatusCode) return true;
            else return false;
        }

        public async Task<List<Order>?> GetOrders()
        {
            HttpResponseMessage response = await AuthorizedGetAsync(GetOrdersEndpoint);

            if (!response.IsSuccessStatusCode) return null;

            SetCancelationToken();

            await using var OrdersJson = await response.Content.ReadAsStreamAsync(cts.Token);

            var Orders = await JsonSerializer.DeserializeAsync<List<Order>?>(
                OrdersJson,
                JsonOptions,
                cts.Token);

            return Orders;
        }
    }
}
