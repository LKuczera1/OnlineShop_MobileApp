namespace OnlineShop_MobileApp.Views;
using OnlineShop_MobileApp.ViewModel;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel();
	}
}