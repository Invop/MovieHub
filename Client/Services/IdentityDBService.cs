using Microsoft.AspNetCore.Components;

namespace MovieHub.Client.Services
{
    public partial class IdentityDbService
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _baseUri;
        private readonly NavigationManager _navigationManager;

        public IdentityDbService(NavigationManager navigationManager, HttpClient httpClient, IConfiguration configuration)
        {
            this._httpClient = httpClient;

            this._navigationManager = navigationManager;
            this._baseUri = new Uri($"{navigationManager.BaseUri}odata/IdentityDB/");
        }

    }
}