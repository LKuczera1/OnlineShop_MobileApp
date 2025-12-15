using OnlineShop_MobileApp.ViewModel;

namespace OnlineShop_MobileApp.Views;

public partial class CatalogView : ContentView
{
	public CatalogView()
	{
		InitializeComponent();
		BindingContext = new CatalogViewModel();
	}
}