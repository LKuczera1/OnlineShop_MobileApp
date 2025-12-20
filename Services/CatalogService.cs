using OnlineShop_MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineShop_MobileApp.Services
{
    public class CatalogService: Service, ICatalogService
    {
        private readonly String get_products_endpoint = "api/Products/page/";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CatalogService(HttpClient client) : base(client)
        {
            
        }

        public async Task<List<Product>> GetProducts(int page = 0, CancellationToken ct = default)
        {
            var url = get_products_endpoint + page.ToString();

            // Twardy timeout, żeby nigdy nie "wisieć"
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(15));

            using var response = await base._httpClient.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead,
                cts.Token
            ).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cts.Token).ConfigureAwait(false);

            var products = await JsonSerializer.DeserializeAsync<List<Product>>(
                stream,
                JsonOptions,
                cts.Token
            ).ConfigureAwait(false);

            return products ?? new List<Product>();
        }
    }
}
