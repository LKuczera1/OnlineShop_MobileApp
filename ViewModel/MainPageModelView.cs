using OnlineShop_MobileApp.Services;
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

        private readonly View _catalog;
        private readonly View _cart;
        private readonly View _orders;
        private readonly View _account;

        private readonly CatalogView _catalogView;

        private View _currentView;
        public View CurrentView
        {
            get => _currentView;
            set { _currentView = value; PropertyChanged?.Invoke(this, new(nameof(CurrentView))); }


        }

        public ICommand SwitchTabCommand { get; }

        

        public MainPageViewModel(CatalogView catalog)
        {
            _catalog = catalog;
            _cart = new CartView();
            _orders = new OrdersView();
            _account = new AccountView();

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
