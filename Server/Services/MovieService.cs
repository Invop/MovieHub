using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;
using System.Net.Http.Headers;

namespace MovieHub.Services
{
    public class MovieService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenManager _tokenManager;
        private const string MovieEndpoint = "/api/movies";
        private const string MovieRatingsEndpoint = "/api/ratings";
        private const string GenresEndpoint = "/api/genres";
        private readonly HttpClient _httpClient;

        public MovieService(IHttpClientFactory httpClientFactory, ITokenManager tokenManager)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
            _httpClient = httpClientFactory.CreateClient("MovieApiClient");
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var cachedToken = await _tokenManager.GetTokenAsync();
            if (!string.IsNullOrEmpty(cachedToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cachedToken);
        }

        public async Task<MovieResponse> GetMovie(string idOrSlug)
        {
            await SetAuthorizationHeaderAsync();
            return await _httpClient.GetFromJsonAsync<MovieResponse>($"{MovieEndpoint}/{idOrSlug}");
        }

        public async Task<MoviesResponse> GetMovies(GetAllMoviesRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var queryString = BuildQueryString(request);
            return await _httpClient.GetFromJsonAsync<MoviesResponse>(MovieEndpoint + queryString);
        }

        public async Task CreateMovie(CreateMovieRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, MovieEndpoint)
            {
                Content = JsonContent.Create(request)
            };
            await _httpClient.SendAsync(requestMessage);
        }

        public async Task UpdateMovie(Guid id, UpdateMovieRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"{MovieEndpoint}/{id}")
            {
                Content = JsonContent.Create(request)
            };
            await _httpClient.SendAsync(requestMessage);
        }

        public async Task DeleteMovie(Guid id)
        {
            await SetAuthorizationHeaderAsync();
            await _httpClient.DeleteAsync($"{MovieEndpoint}/{id}");
        }

        public async Task RateMovie(Guid id, RateMovieRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"{MovieEndpoint}/{id}/ratings")
            {
                Content = JsonContent.Create(request)
            };
            await _httpClient.SendAsync(requestMessage);
        }

        public async Task DeleteRating(Guid id)
        {
            await SetAuthorizationHeaderAsync();
            await _httpClient.DeleteAsync($"{MovieEndpoint}/{id}/ratings");
        }

        public async Task<MovieRatingResponse[]> GetUserRatings()
        {
            await SetAuthorizationHeaderAsync();
            return await _httpClient.GetFromJsonAsync<MovieRatingResponse[]>($"{MovieRatingsEndpoint}/me");
        }

        public async Task<GenreResponse> GetGenre(string idOrName)
        {
            await SetAuthorizationHeaderAsync();
            return await _httpClient.GetFromJsonAsync<GenreResponse>($"{GenresEndpoint}/{idOrName}");
        }

        public async Task<IEnumerable<GenreResponse>> GetAllGenres()
        {
            await SetAuthorizationHeaderAsync();
            return await _httpClient.GetFromJsonAsync<IEnumerable<GenreResponse>>(GenresEndpoint);
        }

        public async Task CreateGenre(CreateGenreRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, GenresEndpoint)
            {
                Content = JsonContent.Create(request)
            };
            await _httpClient.SendAsync(requestMessage);
        }

        public async Task UpdateGenre(int id, UpdateGenreRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"{GenresEndpoint}/{id}")
            {
                Content = JsonContent.Create(request)
            };
            await _httpClient.SendAsync(requestMessage);
        }

        public async Task DeleteGenre(int id)
        {
            await SetAuthorizationHeaderAsync();
            await _httpClient.DeleteAsync($"{GenresEndpoint}/{id}");
        }

        private string BuildQueryString(GetAllMoviesRequest request)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(request.Title))
            {
                queryParams.Add($"title={request.Title}");
            }

            if (request.Year.HasValue)
            {
                queryParams.Add($"year={request.Year}");
            }

            queryParams.Add($"page={request.Page}");
            queryParams.Add($"pageSize={request.PageSize}");

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                queryParams.Add($"sortBy={request.SortBy}");
            }

            return "?" + string.Join("&", queryParams);
        }
    }
}