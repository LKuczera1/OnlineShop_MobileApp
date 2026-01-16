namespace OnlineShop_MobileApp.Models.DTOs
{
    public class LoginRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public void Clear()
        {
            UserName = string.Empty;
            Password = string.Empty;
        }
    }
}
