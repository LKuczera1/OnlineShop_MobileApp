namespace OnlineShop_MobileApp.Views;

public partial class CartView : ContentView
{
    public CartView(CartViewModel cartViewModel)
    {
        InitializeComponent();
        BindingContext = cartViewModel;

    }
}