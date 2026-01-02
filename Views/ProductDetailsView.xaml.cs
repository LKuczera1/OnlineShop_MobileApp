using OnlineShop_MobileApp.ViewModel;

namespace OnlineShop_MobileApp.Views;

public partial class ProductDetailsView : ContentView
{
	public ProductDetailsView(CatalogViewModel catalogViewModel)
	{
		InitializeComponent();
		BindingContext = catalogViewModel;

    }
}