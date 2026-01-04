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

        public ICommand SwitchTabCommand { get; }

        public MainPageViewModel(CatalogView catalog, AccountView account)
        {
            _catalog = catalog;
            _cart = new CartView();
            _orders = new OrdersView();
            _account = account;

            _currentView = _catalog;

            SwitchTabCommand = new Command<string>(tab =>
            {
                CurrentView = tab switch
                {
                    "Catalog" => _catalog,
                    "Cart" => _cart,
                    "Orders" => _orders,
                    "Account" => _account,
                    _ => _catalog
                };
            });
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}