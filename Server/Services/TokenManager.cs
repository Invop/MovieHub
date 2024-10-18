using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace MovieHub.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly SecurityService _securityService;
        private readonly ILogger<TokenManager> _logger;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "CachedToken";

        public TokenManager(IHttpClientFactory clientFactory, SecurityService securityService,
            ILogger<TokenManager> logger, IDistributedCache cache)
        {
            _clientFactory = clientFactory;
            _securityService = securityService;
            _logger = logger;
            _cache = cache;
        }

        public async Task<string> GetTokenAsync()
        {
            if(!_securityService.IsAuthenticated())
                return null;
            var userCache = GenerateUserCacheKey(_securityService.User.Email);
            var cachedToken = await _cache.GetStringAsync($"{CacheKey}:{userCache}");
            if (!string.IsNullOrEmpty(cachedToken) && IsTokenValid(cachedToken))
            {
                return cachedToken;
            }

            if (!IsValidUserEmail())
            {
                return null;
            }

            var client = _clientFactory.CreateClient("TokenClient");
            var requestContent = CreateRequestContent();
            try
            {
                var response = await client.PostAsync("https://localhost:5003/token", requestContent);
                if (!response.IsSuccessStatusCode)
                {
                    await LogErrorResponse(response);
                    return null;
                }

                var token = await response.Content.ReadAsStringAsync();
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(7)
                };
                await _cache.SetStringAsync($"{CacheKey}:{userCache}", token, cacheOptions);
                return token;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Request failed with message: {Message}", e.Message);
                return null;
            }
        }
        
        private bool IsValidUserEmail()
        {
            return !string.IsNullOrWhiteSpace(_securityService.User?.Email);
        }

        private StringContent CreateRequestContent()
        {
            bool isAdmin = _securityService.IsAdministrator();
            bool isTrustedMember = _securityService.IsTrustedMember();

            var request = new
            {
                userid = _securityService.User.Id,
                email = _securityService.User?.Email,
                customClaims = new Dictionary<string, object>
                {
                    { "admin", isAdmin },
                    { "trusted_member", isTrustedMember }
                }
            };

            var jsonRequest = JsonConvert.SerializeObject(request);
            return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        }

        private async Task LogErrorResponse(HttpResponseMessage response)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to retrieve token. Status code: {StatusCode}. Response: {Response}",
                response.StatusCode, errorContent);
        }

        private bool IsTokenValid(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            return jwtToken?.ValidTo > DateTime.UtcNow;
        }
        
        private string GenerateUserCacheKey(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            // Use SHA256 to generate a consistent hash
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(email.ToLowerInvariant());
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }
        }
    }
}