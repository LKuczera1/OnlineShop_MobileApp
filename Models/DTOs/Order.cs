namespace Shopping.Models
{

    public enum OrderStatus
    {
        Paid = 0,
        InRealisation = 1,
        InDelivery = 2,
        Delivered = 3,
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int ClientId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Paid;
        public double TotalPrice { get; set; }
        public DateTime OrderTime { get; set; } = DateTime.Now;
        public DateTime? PackedTime { get; set; } = null;
        public DateTime? SendTime { get; set; } = null;
        public DateTime? DeliveredTime { get; set; } = null;
    }
}


