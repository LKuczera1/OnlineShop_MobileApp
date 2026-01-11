using OnlineShop_MobileApp.GUI_elements;
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

        private Allert _allert = new Allert();

        public Allert allert
        {
            get => _allert;
            set
            {
                _allert = value;
                OnPropertyChanged();
            }
        }

        public ICommand SwitchTabCommand { get; }

        public MainPageViewModel(CatalogView catalog, AccountView account, CartView cart)
        {
            _catalog = catalog;
            _cart = cart;
            _orders = new OrdersView();
            _account = account;

            _currentView = _catalog;

            SwitchTabCommand = new Command<string>(tab =>
            {
                switch(tab)
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
                        break;
                    case "Account":
                        CurrentView = _account;
                        break;
                    default:
                        CurrentView = _catalog;
                        break;
                }
            });
        }

        private void RefreshCurrentView(View view)
        {
            switch (view)
            {
                case CartView cartView when cartView.BindingContext is CartViewModel cartVm:
                    cartVm.Refresh();
                    break;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}