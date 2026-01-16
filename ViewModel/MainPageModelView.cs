using OnlineShop_MobileApp.GUI_elements;
using OnlineShop_MobileApp.Services;
using OnlineShop_MobileApp.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace OnlineShop_MobileApp.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly View _catalog;
        private readonly View _cart;
        private readonly View _orders;
        private readonly View _account;
        private readonly ThemeService _themeService;

        private View _currentView;
        public View CurrentView
        {
            get => _currentView;
            set
            {
                if (ReferenceEquals(_currentView, value)) return;
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private Allert _allert = new Allert();

        private Message _message = new Message();

        public Allert allert
        {
            get => _allert;
            set
            {
                _allert = value;
                OnPropertyChanged();
            }
        }
        public Message message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public ICommand SwitchToLightTheme { get; set; }
        public ICommand SwitchToDarkTheme { get; set; }
        public ICommand SwitchToGoldTheme { get; set; }


        public ICommand SwitchTabCommand { get; }

        public MainPageViewModel(CatalogView catalog, AccountView account, CartView cart, OrdersView orders, ThemeService themeService)
        {
            _catalog = catalog;
            _cart = cart;
            _orders = orders;
            _account = account;
            _themeService = themeService;

            _currentView = _catalog;

            SwitchTabCommand = new Command<string>(tab =>
            {
                switch (tab)
                {
                    case "Catalog":
                        CurrentView = _catalog;
                        break;
                    case "Cart":
                        CurrentView = _cart;
                        RefreshCurrentView(_cart);
                        break;
                    case "Orders":
                        CurrentView = _orders;
                        RefreshCurrentView(_orders);
                        break;
                    case "Account":
                        CurrentView = _account;
                        break;
                    default:
                        CurrentView = _catalog;
                        break;
                }
            });




            SwitchToLightTheme = new Command(() => _themeService.Apply(AppThemeMode.Light));
            SwitchToDarkTheme = new Command(() => _themeService.Apply(AppThemeMode.Dark));
            SwitchToGoldTheme = new Command(() => _themeService.Apply(AppThemeMode.Gold));

        }

        private void RefreshCurrentView(View view)
        {
            switch (view)
            {
                case CartView cartView when cartView.BindingContext is CartViewModel cartVm:
                    cartVm.Refresh();
                    break;
                case OrdersView ordersView when ordersView.BindingContext is OrdersViewModel ordersVm:
                    ordersVm.Refresh();
                    break;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}