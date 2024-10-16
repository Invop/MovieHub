using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MovieHub.Server.Models;

namespace MovieHub.Client.Services
{
    public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly SecurityService _securityService;
        private ApplicationAuthenticationState _authenticationState;

        public ApplicationAuthenticationStateProvider(SecurityService securityService)
        {
            this._securityService = securityService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();

            try
            {
                var state = await GetApplicationAuthenticationStateAsync();

                if (state.IsAuthenticated)
                {
                    identity = new ClaimsIdentity(state.Claims.Select(c => new Claim(c.Type, c.Value)), "MovieHub.Server");
                }
            }
            catch (HttpRequestException ex)
            {
            }

            var result = new AuthenticationState(new ClaimsPrincipal(identity));

            await _securityService.InitializeAsync(result);

            return result;
        }

        private async Task<ApplicationAuthenticationState> GetApplicationAuthenticationStateAsync()
        {
            if (_authenticationState == null)
            {
                _authenticationState = await _securityService.GetAuthenticationStateAsync();
            }

            return _authenticationState;
        }
    }
}