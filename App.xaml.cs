using OnlineShop_MobileApp.Views;

namespace OnlineShop_MobileApp
{
    public partial class App : Application
    {
        public App(MainPage mainPage)
        {
            InitializeComponent();
            base.MainPage = mainPage;
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

#if WINDOWS
        //Based on samsung galaxy g54 screen resolution divided by 2 (1080 x 2340)
        //to recreate the look of a real phone screen as closely as possible
        //Also window height was reduced by 250px to fit in computer screen...

        //Zrobić tu automatyczną konfiguracje z configa

            window.Width = 540;
            window.Height = 930;
            window.Title = "OnlineShopMobileApp";
#endif

            return window;
        }
    }
}