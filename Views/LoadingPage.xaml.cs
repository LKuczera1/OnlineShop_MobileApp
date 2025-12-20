using OnlineShop_MobileApp.ViewModel;

namespace OnlineShop_MobileApp.Views;

public partial class LoadingPage : ContentPage
{
	public LoadingPage(LoadingPageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}