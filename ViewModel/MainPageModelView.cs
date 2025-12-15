using OnlineShop_MobileApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OnlineShop_MobileApp.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private View _currentView;
        public View CurrentView
        {
            get => _currentView;
            set { _currentView = value; PropertyChanged?.Invoke(this, new(nameof(CurrentView))); }
        }

        public ICommand SwitchTabCommand { get; }

        private readonly View _catalog = new CatalogView();
        private readonly View _cart = new CartView();
        private readonly View _orders = new OrdersView();
        private readonly View _account = new AccountView();

        public MainPageViewModel()
        {
            CurrentView = _catalog;

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
    }


}
