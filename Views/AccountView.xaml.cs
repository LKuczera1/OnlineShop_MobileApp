using OnlineShop_MobileApp.Services;
using OnlineShop_MobileApp.ViewModel;

namespace OnlineShop_MobileApp.Views;

public partial class AccountView : ContentView
{
	public AccountView(AccountViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;

    }
}