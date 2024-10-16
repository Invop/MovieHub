namespace MovieHub.Services;

public class TokenManager : ITokenManager
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly SecurityService _securityService;
    private string _cachedToken;

    public TokenManager(IHttpClientFactory clientFactory, SecurityService securityService)
    {
        _clientFactory = clientFactory;
        this._securityService = securityService;
    }

    public async Task<string> GetTokenAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            return _cachedToken;
        }

        var client = _clientFactory.CreateClient("TokenClient");

        var tokenRequest = new
        {
            userid = _securityService.User.Id,
            email = _securityService.User.Email,
            customClaims = new
            {
                admin = true,
                trusted_member = true
            }
        };

        var response = await client.PostAsJsonAsync("/token", tokenRequest);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<string>();
        _cachedToken = tokenResponse;

        return _cachedToken;
    }
}