using OnlineShop_MobileApp.ViewModel;

namespace OnlineShop_MobileApp.Views;

public partial class AccountView : ContentView
{
	public AccountView()
	{
		InitializeComponent();
		BindingContext = new AccountViewModel();
	}
}