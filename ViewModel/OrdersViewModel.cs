using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.ViewModel
{
    using OnlineShop_MobileApp.Services;
    using Shopping.Models;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public class OrdersViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IShoppingService _shoppingService;

        public ObservableCollection<OrderItem> Items { get; } = new();

        private List<Order>? _orders = null;

        public ICommand ChangeOrderDetailsVisiblity { get; }

        public OrdersViewModel(IShoppingService shoppingService)
        {
            _shoppingService = shoppingService;

            ChangeOrderDetailsVisiblity = new Command<OrderItem>(item => ChangeOrderDetailVisiblity(item));
        }

        public async Task Refresh()
        {
            await GetOrderItems();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                OnPropertyChanged(nameof(Items));
            });
        }

        public void ChangeOrderDetailVisiblity(OrderItem item)
        {
            if (item.AreDetailsVisible) item.AreDetailsVisible = false;
            else item.AreDetailsVisible = true;
        }

        public async Task GetOrderItems()
        {
            _orders = await _shoppingService.GetOrders();

            if(_orders==null)
            {
                _orders = new List<Order>();
                Items.Clear();
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Items.Clear();
                foreach (var p in _orders)
                {
                    Items.Add(new OrderItem(p));
                }
            });
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class OrderItem: Order, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _areDetailsVisible = false;

        public string OrderStatusString { get; }

        public OrderItem() { }

        public OrderItem(Order order)
        {
            base.ClientId = order.ClientId;
            base.DeliveredTime = order.DeliveredTime;
            base.OrderId = order.OrderId;
            base.TotalPrice = order.TotalPrice;
            base.Status = order.Status;
            base.SendTime = order.SendTime;
            base.PackedTime = order.PackedTime;
            base.OrderTime = order.OrderTime;
            base.DeliveredTime = order.DeliveredTime;

            switch (order.Status)
            {
                case OrderStatus.Paid:
                    OrderStatusString = "Order has been paid and is waiting for realisation.";
                    break;
                case OrderStatus.InRealisation:
                    OrderStatusString = "Your order is being prepared for shipment";
                    break;
                case OrderStatus.InDelivery:
                    OrderStatusString = "Your order is on its way!";
                    break;
                case OrderStatus.Delivered:
                    OrderStatusString = "Order has been delivered. We hope that your order meets your expectations.";
                    break;
                default:
                    OrderStatusString = "We apologize, but we are unable to verify your order status at this time.";
                    break;

            }
        }

        public bool AreDetailsVisible
        {
            get { return _areDetailsVisible; }
            set { 
                _areDetailsVisible = value;
                OnPropertyChanged();
            }
        }
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
