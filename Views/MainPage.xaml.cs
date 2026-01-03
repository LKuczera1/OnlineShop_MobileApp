namespace OnlineShop_MobileApp.Views;

using OnlineShop_MobileApp.Models;
using OnlineShop_MobileApp.Navigators;
using OnlineShop_MobileApp.ViewModel;

public partial class MainPage : ContentPage, IMainPageNavigator
{
    private readonly MainPageViewModel _mainVm;
    private readonly CatalogViewModel _catalogVm;

    private readonly CatalogView _catalogView;
    private readonly ProductDetailsView _detailsView;

    bool _menuOpen = false;

    public MainPage(
        MainPageViewModel mainPageViewModel,
        CatalogViewModel catalogViewModel,
        CatalogView catalogView,
        ProductDetailsView detailsView)
    {
        InitializeComponent();

        _mainVm = mainPageViewModel;
        BindingContext = _mainVm;

        _catalogVm = catalogViewModel;
        _catalogVm.SetNavigator(this);

        _catalogView = catalogView;
        _catalogView.BindingContext = _catalogVm;

        _detailsView = detailsView;

    }

    async void OnMenuClicked(object sender, EventArgs e)
    {
        if (_menuOpen) await CloseMenu();
        else await OpenMenu();
    }

    async void OnOverlayTapped(object sender, EventArgs e)
    {
        if (_menuOpen) await CloseMenu();
    }

    async Task OpenMenu()
    {
        _menuOpen = true;
        MenuOverlay.IsVisible = true;

        MenuPanel.TranslationX = -MenuPanel.WidthRequest;
        await MenuPanel.TranslateTo(0, 0, 180, Easing.CubicOut);
    }

    async Task CloseMenu()
    {
        _menuOpen = false;
        await MenuPanel.TranslateTo(-MenuPanel.WidthRequest, 0, 160, Easing.CubicIn);
        MenuOverlay.IsVisible = false;
    }

    public void ShowCatalog()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        ((MainPageViewModel)BindingContext).CurrentView = _catalogView);
    }

    public void ShowProductDetails(Product product)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _detailsView.BindingContext = product;
            ((MainPageViewModel)BindingContext).CurrentView = _detailsView;
        });
    }
}