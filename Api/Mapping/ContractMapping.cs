using Bogus;
using MovieHub.Application.Infrastructure.Services;
using MovieHub.Application.Models;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;

namespace MovieHub.Api.Mapping;

public static class ContractMapping
{
    public static async Task<Movie> MapToMovie(this CreateMovieRequest request, IGenreService genreService)
    {
        var movieId = Guid.NewGuid();
        var allGenres = await genreService.GetAllGenresAsync();
        var selectedGenres = allGenres
            .Where(gl => request.Genres.Contains(gl.Id));

        return new Movie
        {
            Id = movieId,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Overview = string.IsNullOrEmpty(request.Overview)
                ? new Faker().Lorem.Paragraphs(min: 1, max: 3)
                : request.Overview,
            PosterBase64 = request.PosterBase64,
            Genres = selectedGenres
                .Select(genre => new Genre 
                { 
                    GenreId = genre.Id, 
                    MovieId = movieId,
                    GenreLookup = genre
                })
                .ToList()
        };
    }

    public static async Task<Movie> MapToMovie(this UpdateMovieRequest request, Guid id,IGenreService genreService)
    {
        var allGenres = await genreService.GetAllGenresAsync();
        var selectedGenres = allGenres
            .Where(gl => request.Genres.Contains(gl.Id));
        return new Movie
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Overview = request.Overview,
            PosterBase64 = request.PosterBase64,
            Genres = selectedGenres
                .Select(genre => new Genre 
                { 
                    GenreId = genre.Id, 
                    MovieId = id,
                    GenreLookup = genre
                })
                .ToList()
        };
    }

    public static MovieResponse MapToResponse(this Movie movie)
    {
        return new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Slug = movie.Slug,
            Rating = movie.Rating,
            UserRating = movie.UserRating,
            YearOfRelease = movie.YearOfRelease,
            Overview = movie.Overview,
            PosterBase64 = movie.PosterBase64,
            Genres = movie.Genres
                .Select(g => g.GenreLookup.Name)
                .ToList()
        };
    }

    public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies, int page, int pageSize, int totalCount)
    {
        return new MoviesResponse
        {
            Items = movies.Select(MapToResponse).ToList(),
            Page = page,
            PageSize = pageSize,
            Total = totalCount
        };
    }

    public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
    {
        return ratings.Select(x => new MovieRatingResponse
        {
            Rating = x.Rating,
            Slug = x.Movie.Slug,
            MovieId = x.MovieId
        });
    }

    public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
    {
        return new GetAllMoviesOptions
        {
            Title = request.Title,
            YearOfRelease = request.Year,
            Genres = request.GenreIds,
            SortField = request.SortBy?.Trim('+', '-'),
            SortOrder = request.SortBy is null ? SortOrder.Unsorted :
                request.SortBy.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending,
            MinRating = request.MinRating,
            MaxRating = request.MaxRating,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public static GenreResponse MapToResponse(this GenreLookup genre)
    {
        return new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name
        };
    }

    public static GenresResponse MapToResponse(this IEnumerable<GenreLookup> genres)
    {
        return new GenresResponse
        {
            Items = genres.Select(MapToResponse).ToList()
        };
    }

    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }
}