using OnlineShop_MobileApp.ViewModel;
namespace OnlineShop_MobileApp.Views;

public partial class OrdersView : ContentView
{
	public OrdersView()
	{
		InitializeComponent();
		BindingContext = new OrdersViewModel();
	}
}