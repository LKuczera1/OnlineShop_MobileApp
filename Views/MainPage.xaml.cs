namespace OnlineShop_MobileApp.Views;
using OnlineShop_MobileApp.ViewModel;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
		BindingContext = new MainPageViewModel();
	}

    bool _menuOpen = false;

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

        // (opcjonalnie) dopasuj szerokoœæ panelu do okna
        // MenuPanel.WidthRequest = Math.Min(360, this.Width * 0.85);

        MenuPanel.TranslationX = -MenuPanel.WidthRequest;
        await MenuPanel.TranslateTo(0, 0, 180, Easing.CubicOut);
    }

    async Task CloseMenu()
    {
        _menuOpen = false;
        await MenuPanel.TranslateTo(-MenuPanel.WidthRequest, 0, 160, Easing.CubicIn);
        MenuOverlay.IsVisible = false;
    }

}