using OnlineShop_MobileApp.Models;
using OnlineShop_MobileApp.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineShop_MobileApp.Services
{
    public class CatalogService : Service, ICatalogService
    {
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
            SetCancelationToken();

            try
            {

                var response = await AuthorizedGetAsync(get_product_by_id + productId.ToString());

                if (!response.IsSuccessStatusCode)
                    return null;

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
            SetCancelationToken();

            var url = get_products_endpoint + page.ToString();

            HttpResponseMessage response = null;

            HttpStatusCode responseStatusCode; // 401, 404 -Not found etc, might be usefull later

            response = await GetAsync(url).ConfigureAwait(false);
            //W services (Wszystko co jest poza UI thread i nie ma własciwości propertychanged) configureAwait() powinno być false

            try
            {
            }
            catch (Exception ex)
            {

            }

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
            SetCancelationToken();

            var url = get_number_of_pages;

            HttpResponseMessage response = null;

            HttpStatusCode responseStatusCode; // 401, 404 -Not found etc, might be usefull later

            response = await GetAsync(url).ConfigureAwait(false);
            //W services (Wszystko co jest poza UI thread i nie ma własciwości propertychanged) configureAwait() powinno być false


            try
            {
            }
            catch (Exception ex)
            {

            }

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

                var temp = response.Content.ReadAsStringAsync();


                return int.Parse(temp.Result);
            }

            return 0;
        }

        public async Task<byte[]?> LoadProductPicture(int id)
        {
            SetCancelationToken();

            var url = get_product_picture + id.ToString();

            HttpResponseMessage response = null;

            HttpStatusCode responseStatusCode; // 401, 404 -Not found etc, might be usefull later

            response = await GetAsync(url).ConfigureAwait(false);
            //W services (Wszystko co jest poza UI thread i nie ma własciwości propertychanged) configureAwait() powinno być false


            try
            {
            }
            catch (Exception ex)
            {

            }

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

                return await response.Content.ReadAsByteArrayAsync(cts.Token);
            }

            return null;
        }

        public async Task<byte[]?> LoadProductThumbnail(int id)
        {
            SetCancelationToken();

            var url = get_product_thumbnail + id.ToString();

            HttpResponseMessage response = null;

            HttpStatusCode responseStatusCode; // 401, 404 -Not found etc, might be usefull later

            response = await GetAsync(url).ConfigureAwait(false);
            //W services (Wszystko co jest poza UI thread i nie ma własciwości propertychanged) configureAwait() powinno być false


            try
            {
            }
            catch (Exception ex)
            {

            }

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

                return await response.Content.ReadAsByteArrayAsync(cts.Token);
            }

            return null;
        }
    }
}
