using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services.Authentication
{
    public record AuthSession(string AccessToken, DateTimeOffset ExpiresAtUtc);

    public interface ITokenStore
    {
        Task<AuthSession?> GetAsync();
        Task SetAsync(AuthSession session);
        Task ClearAsync();
    }
}
