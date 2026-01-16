namespace OnlineShop_MobileApp.Models.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
    }
}
