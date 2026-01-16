namespace OnlineShop_MobileApp.Models.DTOs
{
    internal class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string ExpiresAt { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int PrivilegeLevel { get; set; }
    }
}