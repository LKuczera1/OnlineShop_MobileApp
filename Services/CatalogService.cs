using OnlineShop_MobileApp.Models;
using OnlineShop_MobileApp.Services.Authentication;
using System.Net;
using System.Text.Json;

namespace OnlineShop_MobileApp.Services
{
    public class CatalogService : Service, ICatalogService
    {
        //ToDo: Move endpoints from class to Config.json
        private readonly String get_products_endpoint = "api/Products/page/";
        private readonly String get_number_of_pages = "api/Products/numberOfProducts";
        private readonly String get_product_thumbnail = "/api/Products/thumbnail/";
        private readonly String get_product_picture = "/api/Products/image/";
        private readonly String get_product_by_id = "/api/Products/";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CatalogService(HttpClient client, ITokenStore tokenStore) : base(client, tokenStore)
        {

        }

        public async Task<Product?> GetProduct(int productId)
        {
            try
            {
                var response = await AuthorizedGetAsync(get_product_by_id + productId.ToString());

                if (!response.IsSuccessStatusCode)
                    return null;

                SetCancelationToken();

                await using var prodJson = await response.Content.ReadAsStreamAsync(cts.Token).ConfigureAwait(false);

                var product = await JsonSerializer.DeserializeAsync<Product>(
                    prodJson,
                    JsonOptions,
                    cts.Token
                ).ConfigureAwait(false);

                return product;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Product>> GetProducts(int page = 0)
        {
            var url = get_products_endpoint + page.ToString();

            HttpResponseMessage? response = null;

            HttpStatusCode responseStatusCode;

            response = await GetAsync(url).ConfigureAwait(false);

            if (response != null)
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    responseStatusCode = response.StatusCode;
                }
                catch
                {

                }

                SetCancelationToken();

                await using var stream = await response.Content.ReadAsStreamAsync(cts.Token).ConfigureAwait(false);

                var products = await JsonSerializer.DeserializeAsync<List<Product>>(
                    stream,
                    JsonOptions,
                    cts.Token
                ).ConfigureAwait(false);

                return products ?? new List<Product>();
            }

            return null;
        }

        public async Task<int> GetNumberOfPages()
        {

            var url = get_number_of_pages;

            HttpResponseMessage response = null;

            HttpStatusCode responseStatusCode;

            response = await GetAsync(url).ConfigureAwait(false);

            if (response != null)
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    responseStatusCode = response.StatusCode;
                }
                catch
                {
                    //Unable to connect - throw an exception
                }

                var temp = response.Content.ReadAsStringAsync();


                return int.Parse(temp.Result);
            }

            return 0;
        }

        public async Task<byte[]?> LoadProductPicture(int id)
        {
            var url = get_product_picture + id.ToString();

            HttpResponseMessage response = null;

            HttpStatusCode responseStatusCode;

            response = await GetAsync(url).ConfigureAwait(false);

            if (response != null)
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    responseStatusCode = response.StatusCode;
                }
                catch
                {
                    //Unable to connect - throw an exception
                }

                SetCancelationToken();

                return await response.Content.ReadAsByteArrayAsync(cts.Token);
            }

            return null;
        }

        public async Task<byte[]?> LoadProductThumbnail(int id)
        {
            var url = get_product_thumbnail + id.ToString();

            HttpResponseMessage response = null;

            HttpStatusCode responseStatusCode;

            response = await GetAsync(url).ConfigureAwait(false);

            if (response != null)
            {
                try
                {
                    response.EnsureSuccessStatusCode();
                    responseStatusCode = response.StatusCode;
                }
                catch
                {
                    //Unable to connect - throw an exception
                }
                //---------------------------------------------------

                SetCancelationToken();

                return await response.Content.ReadAsByteArrayAsync(cts.Token);
            }

            return null;
        }
    }
}
