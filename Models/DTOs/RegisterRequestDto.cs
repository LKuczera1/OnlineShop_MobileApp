namespace OnlineShop_MobileApp.Models.DTOs
{
    public class RegisterRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public void Clear()
        {
            UserName = string.Empty;
            Password = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Address = string.Empty;
            City = string.Empty;
        }
    }
}
