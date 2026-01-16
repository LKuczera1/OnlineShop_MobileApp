using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using OnlineShop_MobileApp.Services;
using OnlineShop_MobileApp.Services.Authentication;
using OnlineShop_MobileApp.Services.Resolver;
using OnlineShop_MobileApp.ViewModel;
using OnlineShop_MobileApp.Views;
using OnlineShopMobileApp.Configuration;

//Showin app window in center of screen
#if WINDOWS
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;
#endif

namespace OnlineShop_MobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Configuration config = new Configuration();
            config.LoadConfiguration();

            var builder = MauiApp.CreateBuilder();

            //Authentication DI
            builder.Services.AddHttpClient("Identity", c =>
            {
                c.BaseAddress = new Uri(config.Properties.services.identity);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                //Android blocks insecure connections by default, this handler is a workaround (related to the https protocol and
                // lack of ssl)
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (m, cert, chain, err) => true;
                return handler;
            });

            builder.Services.AddSingleton<ITokenStore>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = factory.CreateClient("Identity");

                var refreshEndpoint = config.Properties.services.IdentityEndpoints["refreshJWT"];
                return new SecureTokenStore(httpClient, refreshEndpoint);
            });

            //Http clients for services
            builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
            {
                c.BaseAddress = new Uri(config.Properties.services.catalog);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                //Android blocks insecure connections by default, this handler is a workaround (related to the https protocol)
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => true;
                return handler;
            });

            builder.Services.AddHttpClient<IIdentityService, IdentityService>(c =>
            {
                c.BaseAddress = new Uri(config.Properties.services.identity);

            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                //Android blocks insecure connections by default, this handler is a workaround (related to the https protocol)
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                return handler;
            });

            builder.Services.AddHttpClient<IShoppingService, ShoppingService>(c =>
            {
                c.BaseAddress = new Uri(config.Properties.services.shopping);

            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                //Android blocks insecure connections by default, this handler is a workaround (related to the https protocol)
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                return handler;
            });

            //Services DI

            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<ThemeService>();

            builder.Services.AddTransient<IServicesResolver, ServicesResolver>();

            builder.Services.AddSingleton<CatalogViewModel>();
            builder.Services.AddSingleton<CatalogView>();

            builder.Services.AddSingleton<CartViewModel>();
            builder.Services.AddSingleton<CartView>();

            builder.Services.AddSingleton<ProductDetailsView>();

            builder.Services.AddSingleton<AccountViewModel>();
            builder.Services.AddSingleton<AccountView>();

            builder.Services.AddSingleton<OrdersViewModel>();
            builder.Services.AddSingleton<OrdersView>();

            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();


            //--Centering app window on windows (run on windows machine)
#if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
{
    events.AddWindows(w =>
    {
        w.OnWindowCreated(window =>
        {
            IntPtr hwnd = WindowNative.GetWindowHandle(window);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);

            var appWindow = AppWindow.GetFromWindowId(windowId);

            // docelowy rozmiar
            var width = 540;
            var height = 930;
            //appWindow.Resize(new SizeInt32(width, height));

            // wylicz środek "work area" (bez paska zadań)
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            var wa = displayArea.WorkArea;

            int x = wa.X + (wa.Width - width) / 2;
            int y = wa.Y + (wa.Height - height) / 2;

            appWindow.Move(new PointInt32(x, y));
        });
    });
});
#endif
            //---------------------------------------------------------------------------------------

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("calibrib.ttf", "CalibriB");
                    fonts.AddFont("calibri.ttf", "Calibri");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

    }
}
