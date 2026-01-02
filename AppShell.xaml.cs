using OnlineShop_MobileApp.Views;

namespace OnlineShop_MobileApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ProductDetailsView), typeof(ProductDetailsView));
        }
    }
}
