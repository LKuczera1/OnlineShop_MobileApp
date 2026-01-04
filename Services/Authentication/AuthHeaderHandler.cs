using OnlineShop_MobileApp.Services.Authentication;
using System.Net.Http.Headers;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenStore _tokenStore;

    public AuthHeaderHandler(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var session = await _tokenStore.GetAsync();

        if (session is not null)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", session.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}