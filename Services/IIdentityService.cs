using OnlineShop_MobileApp.Models.DTOs;

namespace OnlineShop_MobileApp.Services
{
    public class RegisterResponseDto
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;

        public void RequestDenied(string? message)
        {
            if (message == null) Message = "Unknown error.";
            else Message = message;

            Success = false;
        }
        public void RequestAccepted(string? message)
        {
            if (message == null) Message = string.Empty;
            else Message = message;

            Success = true;
        }
    }
    public interface IIdentityService
    {
        public Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);

        public Task<UserDataDto?> GetUserData(int userId);
        public Task<RegisterResponseDto> CreateNewAccount(RegisterRequestDto regRequest);
        public void LogOut();
    }
}
