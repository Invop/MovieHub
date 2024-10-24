using Newtonsoft.Json;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using MovieHub.Contracts.Requests.Genre;
using MovieHub.Contracts.Requests.Movie;
using MovieHub.Contracts.Requests.Rating;
using MovieHub.Contracts.Responses.Genre;
using MovieHub.Contracts.Responses.Movie;
using MovieHub.Contracts.Responses.Rating;

namespace MovieHub.Services
{
    public class MovieService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenManager _tokenManager;
        private readonly ILogger<MovieService> _logger;
        private const string MovieEndpoint = "/api/movies";
        private const string MovieRatingsEndpoint = "/api/ratings";
        private const string GenresEndpoint = "/api/genres";
        private readonly HttpClient _httpClient;

        public MovieService(IHttpClientFactory httpClientFactory, ITokenManager tokenManager,
            ILogger<MovieService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tokenManager = tokenManager;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("MovieApiClient");
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var cachedToken = await _tokenManager.GetTokenAsync();
            if (!string.IsNullOrEmpty(cachedToken))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cachedToken);
        }

        public async Task<ServiceResponse<MovieResponse>> GetMovie(Guid idOrSlug)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"{MovieEndpoint}/{idOrSlug}");

            var errorMessages = await LogIfError(response, $"Failed to get movie {idOrSlug}");
            var data = errorMessages.Length == 0 ? await response.Content.ReadFromJsonAsync<MovieResponse>() : null;

            return new ServiceResponse<MovieResponse>
            {
                Data = data,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<MoviesResponse>> GetMovies(GetAllMoviesRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var queryString = BuildGetAllMoviesQueryString(request);
            var response = await _httpClient.GetAsync(MovieEndpoint + queryString);

            var errorMessages = await LogIfError(response, $"Failed to get movies with request {request}");
            var data = errorMessages.Length == 0 ? await response.Content.ReadFromJsonAsync<MoviesResponse>() : null;

            return new ServiceResponse<MoviesResponse>
            {
                Data = data,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<bool>> CreateMovie(CreateMovieRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, MovieEndpoint)
            {
                Content = JsonContent.Create(request)
            };
            var response = await _httpClient.SendAsync(requestMessage);

            var errorMessages = await LogIfError(response, $"Failed to create movie with request {request}");
            return new ServiceResponse<bool>
            {
                Data = errorMessages.Length == 0,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<bool>> UpdateMovie(Guid id, UpdateMovieRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"{MovieEndpoint}/{id}")
            {
                Content = JsonContent.Create(request)
            };
            var response = await _httpClient.SendAsync(requestMessage);

            var errorMessages = await LogIfError(response, $"Failed to update movie {id} with request {request}");
            return new ServiceResponse<bool>
            {
                Data = errorMessages.Length == 0,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<bool>> DeleteMovie(Guid id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"{MovieEndpoint}/{id}");

            var errorMessages = await LogIfError(response, $"Failed to delete movie {id}");
            return new ServiceResponse<bool>
            {
                Data = errorMessages.Length == 0,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<bool>> RateMovie(Guid id, RateMovieRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"{MovieEndpoint}/{id}/ratings")
            {
                Content = JsonContent.Create(request)
            };
            var response = await _httpClient.SendAsync(requestMessage);

            var errorMessages = await LogIfError(response, $"Failed to rate movie {id} with request {request}");
            return new ServiceResponse<bool>
            {
                Data = errorMessages.Length == 0,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<bool>> DeleteRating(Guid id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"{MovieEndpoint}/{id}/ratings");

            var errorMessages = await LogIfError(response, $"Failed to delete rating for movie {id}");
            return new ServiceResponse<bool>
            {
                Data = errorMessages.Length == 0,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<MovieRatingResponse[]>> GetUserRatings()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"{MovieRatingsEndpoint}/me");

            var errorMessages = await LogIfError(response, "Failed to get user ratings");
            var data = errorMessages.Length == 0
                ? await response.Content.ReadFromJsonAsync<MovieRatingResponse[]>()
                : null;

            return new ServiceResponse<MovieRatingResponse[]>
            {
                Data = data,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<GenreResponse>> GetGenre(string idOrName)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync($"{GenresEndpoint}/{idOrName}");

            var errorMessages = await LogIfError(response, $"Failed to get genre {idOrName}");
            var data = errorMessages.Length == 0 ? await response.Content.ReadFromJsonAsync<GenreResponse>() : null;

            return new ServiceResponse<GenreResponse>
            {
                Data = data,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<GenresResponse>> GetAllGenresAsync()
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.GetAsync(GenresEndpoint);

            var errorMessages = await LogIfError(response, "Failed to get all genres");
            var data = errorMessages.Length == 0 ? await response.Content.ReadFromJsonAsync<GenresResponse>() : null;

            return new ServiceResponse<GenresResponse>
            {
                Data = data,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<bool>> CreateGenre(CreateGenreRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, GenresEndpoint)
            {
                Content = JsonContent.Create(request)
            };
            var response = await _httpClient.SendAsync(requestMessage);

            var errorMessages = await LogIfError(response, $"Failed to create genre with request {request}");
            return new ServiceResponse<bool>
            {
                Data = errorMessages.Length == 0,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<bool>> UpdateGenre(int id, UpdateGenreRequest request)
        {
            await SetAuthorizationHeaderAsync();
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"{GenresEndpoint}/{id}")
            {
                Content = JsonContent.Create(request)
            };
            var response = await _httpClient.SendAsync(requestMessage);

            var errorMessages = await LogIfError(response, $"Failed to update genre {id} with request {request}");
            return new ServiceResponse<bool>
            {
                Data = errorMessages.Length == 0,
                ErrorMessages = errorMessages.ToList()
            };
        }

        public async Task<ServiceResponse<bool>> DeleteGenre(int id)
        {
            await SetAuthorizationHeaderAsync();
            var response = await _httpClient.DeleteAsync($"{GenresEndpoint}/{id}");

            var errorMessages = await LogIfError(response, $"Failed to delete genre {id}");
            return new ServiceResponse<bool>
            {
                Data = errorMessages.Length == 0,
                ErrorMessages = errorMessages.ToList()
            };
        }

        private async Task<string[]> LogIfError(HttpResponseMessage response, string message)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var errorMessages = new List<string>();

                // Try to deserialize as a dictionary of lists (errors by property name)
                try
                {
                    var validationFailureResponseDict =
                        JsonConvert.DeserializeObject<ValidationFailureResponseDict>(responseContent);

                    if (validationFailureResponseDict?.Errors != null)
                    {
                        foreach (var error in validationFailureResponseDict.Errors)
                        {
                            foreach (var errorDetail in error.Value)
                            {
                                var logMessage = $"Error in field '{error.Key}': {errorDetail}";
                                _logger.LogError($"{message}, Status Code: {response.StatusCode}, {logMessage}");
                                errorMessages.Add(logMessage);
                            }
                        }

                        return errorMessages.ToArray();
                    }
                }
                catch (JsonException)
                {
                }

                // Try to deserialize as a list of validation errors
                try
                {
                    var validationFailureResponseArray =
                        JsonConvert.DeserializeObject<ValidationFailureResponseArray>(responseContent);

                    if (validationFailureResponseArray?.Errors != null)
                    {
                        foreach (var error in validationFailureResponseArray.Errors)
                        {
                            var logMessage = $"Error in field '{error.PropertyName}': {error.Message}";
                            _logger.LogError($"{message}, Status Code: {response.StatusCode}, {logMessage}");
                            errorMessages.Add(logMessage);
                        }

                        return errorMessages.ToArray();
                    }
                }
                catch (JsonException)
                {
                }

                // If both deserialization attempts fail
                _logger.LogError($"{message}, Status Code: {response.StatusCode}, Response: {responseContent}");
                _logger.LogError("Failed to deserialize validation error response.");
                errorMessages.Add("Failed to deserialize validation error response.");

                return errorMessages.ToArray();
            }

            return Array.Empty<string>();
        }

        private string BuildGetAllMoviesQueryString(GetAllMoviesRequest request)
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

            if (request.MinRating.HasValue)
            {
                queryParams.Add($"minRating={request.MinRating}");
            }

            if (request.MaxRating.HasValue)
            {
                queryParams.Add($"maxRating={request.MaxRating}");
            }

            if (request.GenreIds != null && request.GenreIds.Any())
            {
                queryParams.AddRange(request.GenreIds.Select(genreId => $"genreIds={genreId}"));
            }

            queryParams.Add($"page={request.Page}");
            queryParams.Add($"pageSize={request.PageSize}");

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                queryParams.Add($"sortBy={Uri.EscapeDataString(request.SortBy)}");
            }

            return "?" + string.Join("&", queryParams);
        }
    }

    public class ValidationFailureResponseDict
    {
        public Dictionary<string, List<string>> Errors { get; set; }
    }

    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string Message { get; set; }
    }

    public class ValidationFailureResponseArray
    {
        public List<ValidationError> Errors { get; set; }
    }

    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public bool IsSuccess => ErrorMessages == null || !ErrorMessages.Any();
    }
}