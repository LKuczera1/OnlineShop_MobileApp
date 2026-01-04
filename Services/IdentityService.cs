using OnlineShop_MobileApp.Services.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop_MobileApp.Services
{
    public class IdentityService: Service, IIdentityService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenStore _tokenStore;

        public IdentityService(HttpClient client, IHttpClientFactory httpClientFactory, ITokenStore tokenStore) : base(client, tokenStore)
        {
            _httpClientFactory = httpClientFactory;
            _tokenStore = tokenStore;
        }

        public async Task LoginAsync(string login, string password)
        {
            var client = _httpClientFactory.CreateClient("Identity");

            // TODO: dopasuj do swojego DTO
            var response = await client.PostAsJsonAsync("/auth/login", new { login, password });

            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<LoginResponseDto>()
                      ?? throw new Exception("Empty login response");

            // dto.AccessToken + dto.ExpiresInSeconds (przykład)
            var expiresAt = DateTimeOffset.UtcNow.AddSeconds(dto.ExpiresInSeconds);

            await _tokenStore.SetAsync(new AuthSession(dto.AccessToken, expiresAt));
        }
    }
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = "";
        public int ExpiresInSeconds { get; set; }
    }
}
