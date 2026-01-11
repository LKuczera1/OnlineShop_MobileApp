using OnlineShop_MobileApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services.Resolver
{
    internal class ServicesResolver :IServicesResolver
    {
        private readonly ICatalogService _catalogService;
        private readonly IIdentityService _identityService;
        private readonly IShoppingService _shoppingService;

        public ServicesResolver(ICatalogService catalog, IIdentityService identityService, IShoppingService shoppingService)
        {
            _catalogService = catalog;
            _identityService = identityService;
            _shoppingService = shoppingService;
        }

        public async Task<Product?> ResolveForProduct(int productId)
        {
            try
            {
                var temp = await _catalogService.GetProduct(productId);

                return await _catalogService.GetProduct(productId);
            }
            catch (Exception ex)
            {

            }

            return null;
        }
        public async Task<byte[]?> ResolveForProductThumbnail(int productId)
        {
            try
            {
                return await _catalogService.LoadProductThumbnail(productId);
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public async Task<bool> ResolveInsertItemIntoCart(int productId)
        {
            return await _shoppingService.InsertItemToCart(productId);
        }
    }
}
