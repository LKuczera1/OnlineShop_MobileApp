using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services.Authentication
{
    public record AuthSession
    {
        public AuthSession(string? accesToken = null, DateTimeOffset? expiresAt= null)
        {
            AccessToken = accesToken;
            ExpiresAtUtc = expiresAt;
        }
        public string? AccessToken { get; set; } = string.Empty;
        public DateTimeOffset? ExpiresAtUtc { get; set; } = null;
    }

    public sealed class TokenRefreshException : Exception
    {
        public TokenRefreshException(string message, Exception? inner = null)
            : base(message, inner) { }
    }

    public interface ITokenStore
    {
        public bool IsUserLoggedIn
        {
            get;
        }
        public Task<AuthSession?> GetAsync();
        Task SetAsync(AuthSession session, string login);
        Task ClearAsync();
        /// <summary>
        /// This method checks checks activity status for JWT token and refreshes it 5 minutes before expiration time.
        /// It's obligatory to use this method every time request requiring authentication is send.
        /// </summary>
        /// <returns><c>true</c> if user is logged in or <c>false</c> if user is not logged in</returns>
        /// <exception cref="TokenRefreshException"></exception>
        public Task<bool> EnsureTokenActivity();
        public Task SetAuthSession(AuthSession session, string login);
    }
}
