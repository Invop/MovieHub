using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MovieHub.Services
{
    public class TokenManager : ITokenManager
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly SecurityService _securityService;
        private readonly ILogger<TokenManager> _logger;
        private string _cachedToken;

        public TokenManager(IHttpClientFactory clientFactory, SecurityService securityService, ILogger<TokenManager> logger)
        {
            _clientFactory = clientFactory;
            _securityService = securityService;
            _logger = logger;
        }

        public async Task<string> GetTokenAsync()
        {
            if (_cachedToken != null && IsTokenValid(_cachedToken))
            {
                return _cachedToken;
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
                _cachedToken = token;
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
            bool isAdmin = _securityService.User.Roles?.Any(role =>
                string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase)) ?? false;
            bool isTrustedMember = _securityService.User.Roles?.Any(role =>
                string.Equals(role.Name, "Trusted_member", StringComparison.OrdinalIgnoreCase)) ?? false;

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

    }
}