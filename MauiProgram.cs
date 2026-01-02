using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using OnlineShop_MobileApp.Services;
using OnlineShop_MobileApp.ViewModel;
using OnlineShop_MobileApp.Views;
using OnlineShopMobileApp.Configuration;
using Paket;

//Zeby pokazywało apke w sensownym miejscu
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

            /* Also works but probably worse
            builder.Services.AddSingleton<ICatalogService, CatalogService>(c =>
            {
                var http = new HttpClient
                {
                    BaseAddress = new Uri("https://api1.twojadomena.pl/")
                };
                return new CatalogService(http);
            });*/


            builder.Services.AddSingleton<ICatalogService, CatalogService>();
            builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
            {
                c.BaseAddress = new Uri(config.Properties.services.catalog);

            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                //Android blocks insecure connections by default, this handler is a workaround (related to the https protocol)
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                return handler;
            });

            /*
            builder.Services.AddHttpClient<IOrdersService, OrdersService>(c =>
            {
                c.BaseAddress = new Uri("https://api2.twojadomena.pl/");
            });

            builder.Services.AddHttpClient<IAuthService, AuthService>(c =>
            {
                c.BaseAddress = new Uri("https://auth.twojadomena.pl/");
            });
            */

            //Rejestracja serwisu - DI
            //Lepsza praktyka jest dodanie ICatalogService - pozwala na mockowanie
            //builder.Services.AddSingleton<ICatalogService, CatalogService>();
            //builder.Services.AddSingleton<CatalogService>();

            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<CatalogViewModel>();
            builder.Services.AddSingleton<CatalogView>();
            builder.Services.AddSingleton<ProductDetailsView>();


            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();

            //----------------------------------------------------------------------------------------
            //------------------Centering app window on windows machine
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
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

    }
}
