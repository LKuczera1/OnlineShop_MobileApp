namespace OnlineShop_MobileApp.Views;

using Microsoft.Maui.Controls;
using OnlineShop_MobileApp.Models;
using OnlineShop_MobileApp.Navigators;
using OnlineShop_MobileApp.ViewModel;

public partial class MainPage : ContentPage, IMainPageNavigator
{
    private readonly MainPageViewModel _mainVm;
    private readonly CatalogViewModel _catalogVm;
    private readonly AccountViewModel _accountVm;
    private readonly CartViewModel _cartVm;

    private readonly CatalogView _catalogView;
    private readonly ProductDetailsView _detailsView;
    private readonly AccountView _accountView;
    private readonly CartView _cartView;

    bool _menuOpen = false;

    public MainPage(
        MainPageViewModel mainPageViewModel,
        CatalogViewModel catalogViewModel,
        AccountViewModel accountViewModel,
        CatalogView catalogView,
        ProductDetailsView detailsView,
        AccountView accountView,
        CartViewModel cartViewModel,
        CartView cartView)
    {
        InitializeComponent();

        _mainVm = mainPageViewModel;
        BindingContext = _mainVm;

        _catalogVm = catalogViewModel;
        _catalogVm.SetNavigator(this);

        _catalogView = catalogView;
        _catalogView.BindingContext = _catalogVm;

        _accountVm = accountViewModel;
        _accountView = accountView;
        _accountView.BindingContext = _accountVm;
        _accountVm.SetNavigator(this);

        _cartVm = cartViewModel;
        _cartView = cartView;
        _cartView.BindingContext = _cartVm;

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

    public void ShowAllert(string title, string message)
    {
        CreateNewAllertBanner(title, message);
    }

    public void RedirectToLoginPage()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ((MainPageViewModel)BindingContext).CurrentView = _accountView;
        });
    }

    //-----------

    private async Task CreateNewAllertBanner(string title, string message)
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            _mainVm.allert = new GUI_elements.Allert(title, message);
            await AnimateAllertbanner();
        });
    }

    private async Task AnimateAllertbanner()
    {
        MessageBanner.IsVisible = true;

        await Task.Yield();

        await MessageBanner.TranslateTo(0, _mainVm.allert.onViewPosition, ((uint)_mainVm.allert.animationLength), Easing.SpringOut);
        await Task.Delay(_mainVm.allert.onScreenTime);
        await MessageBanner.TranslateTo(0, _mainVm.allert.hiddenPosition, ((uint)_mainVm.allert.animationLength), Easing.Linear);

        _mainVm.allert.IsAllertVisible = false;
        MessageBanner.IsVisible = false;
    }
}