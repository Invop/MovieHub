using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace MovieHub.Services;

public class AuthenticatedHttpClientHandler : DelegatingHandler
{
    private readonly HttpClient _httpClient;
    private readonly SecurityService _securityService;
    private static string _cachedToken;
    private static readonly SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

    public AuthenticatedHttpClientHandler(HttpClient httpClient, SecurityService securityService)
    {
        _httpClient = httpClient;
        _securityService = securityService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await GetTokenAsync();

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    // Updated GetTokenAsync logic
    public async Task<string> GetTokenAsync()
    {
        // Check if cached token is still valid
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(_cachedToken);
            var expiryTimeText = jwt.Claims.Single(claim => claim.Type == "exp").Value;
            var expiryDateTime = UnixTimeStampToDateTime(int.Parse(expiryTimeText));

            if (expiryDateTime > DateTime.UtcNow)
            {
                return _cachedToken;
            }
        }

        // Acquire lock to avoid race conditions during token retrieval
        await Lock.WaitAsync();
        try
        {
            // If the cached token is invalid or expired, request a new one
            var response = await _httpClient.PostAsJsonAsync("https://localhost:5003/token", new
            {
                userid = _securityService.User.Id,
                email = _securityService.User.Email,
                customClaims = new Dictionary<string, object>
                {
                    { "admin", true },
                    { "trusted_member", true }
                }
            });

            response.EnsureSuccessStatusCode(); // Ensure the response was successful

            var newToken = await response.Content.ReadAsStringAsync();
            _cachedToken = newToken; // Cache the new token
            return newToken;
        }
        finally
        {
            Lock.Release(); // Release the lock
        }
    }

    // Convert Unix timestamp to DateTime
    private static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime;
    }
}