using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.ViewModel
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public class OrdersViewModel
    {
        public ObservableCollection<OrderItem> Items { get; } = new();

        public ICommand CheckOrderStatusCommand { get; }

        public OrdersViewModel()
        {
            CheckOrderStatusCommand = new Command<OrderItem>(item =>
            {
                if (item == null) return;

                // na start: np. popup / alert / log
                Application.Current?.MainPage?.DisplayAlert(
                    "Status zamówienia",
                    $"Zamówienie: {item.OrderNumber}\nStatus: {item.Status}",
                    "OK");
            });

            // MOCK
            Items.Add(new OrderItem { OrderNumber = "ORD-0001", DateText = "2025-12-15", Status = "Processing" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
            Items.Add(new OrderItem { OrderNumber = "ORD-0002", DateText = "2025-12-10", Status = "Shipped" });
        }
    }

    public class OrderItem
    {
        public string? OrderNumber { get; set; }
        public string? DateText { get; set; }
        public string? Status { get; set; }
    }

}
