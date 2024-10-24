using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MovieHub.Application.Data;
using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly MovieHubDbContext _context;

    public MovieRepository(MovieHubDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _context.Movies.AddAsync(movie, token);
        return await SaveChangesAsync(token);
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        var movie = await GetMovieByPredicate(m => m.Id == id, token);
        return await EnrichMovieWithRatings(movie, userId);
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken token = default)
    {
        var movie = await GetMovieByPredicate(m => m.Slug == slug, token);
        return await EnrichMovieWithRatings(movie, userId);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken token = default)
    {
        var query = BuildBaseMovieQuery();
        query = ApplyFilters(query, options);
        query = ApplySorting(query, options);
    
        var movieList = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync(token);

        var enrichedMovies = new List<Movie>();
    
        foreach (var movie in movieList)
        {
            var enrichedMovie = await EnrichMovieWithRatings(movie, options.UserId);
            if (enrichedMovie != null)
            {
                enrichedMovies.Add(enrichedMovie);
            }
        }

        return enrichedMovies;
    }


    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        var existingMovie = await _context.Movies
            .Include(m => m.Genres)
            .Include(m => m.MovieActors)
            .FirstOrDefaultAsync(m => m.Id == movie.Id, token);

        if (existingMovie == null) return false;

        UpdateMovieProperties(existingMovie, movie);
        return await SaveChangesAsync(token);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var movie = await _context.Movies.FindAsync([id], token);
        if (movie == null) return false;

        _context.Movies.Remove(movie);
        return await SaveChangesAsync(token);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default) =>
        await _context.Movies.AnyAsync(m => m.Id == id, token);

    public async Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
    {
        var query = _context.Movies.AsQueryable();
        query = ApplyBasicFilters(query, title, yearOfRelease);
        return await query.CountAsync(token);
    }

    #region Private Helper Methods

    private IQueryable<Movie> BuildBaseMovieQuery() =>
        _context.Movies
            .Include(m => m.Genres)
            .ThenInclude(g => g.GenreLookup)
            .Include(m => m.MovieActors)
            .ThenInclude(ma => ma.Actor)
            .Include(m => m.Ratings)
            .AsSplitQuery();

    private async Task<Movie?> GetMovieByPredicate(Expression<Func<Movie, bool>> predicate,CancellationToken token)
    {
        return await BuildBaseMovieQuery()
            .FirstOrDefaultAsync(predicate, token);
    }

    private static async Task<Movie?> EnrichMovieWithRatings(Movie? movie, Guid? userId)
    {
        if (movie == null) return null;

        movie.Rating = movie.Ratings.Any() 
            ? MathF.Round(movie.Ratings.Average(r => (float)r.Rating), 1) 
            : null;
            
        movie.UserRating = userId.HasValue 
            ? movie.Ratings.FirstOrDefault(r => r.UserId == userId.Value)?.Rating 
            : null;

        return movie;
    }

    private static IQueryable<Movie> ApplyFilters(IQueryable<Movie> query, GetAllMoviesOptions options)
    {
        if (!string.IsNullOrEmpty(options.Title))
            query = query.Where(m => EF.Functions.Like(m.Title, $"%{options.Title}%"));

        if (options.YearOfRelease.HasValue)
            query = query.Where(m => m.YearOfRelease == options.YearOfRelease.Value);

        if (options.Genres?.Any() == true)
            query = query.Where(m => options.Genres.All(genreId => 
                m.Genres.Any(g => g.GenreId == genreId)));

        query = ApplyRatingFilters(query, options);
        query = ApplyActorFilters(query, options);

        return query;
    }

    private static IQueryable<Movie> ApplyRatingFilters(IQueryable<Movie> query, GetAllMoviesOptions options)
    {
        if (options.MinRating.HasValue)
            query = query.Where(m => m.Ratings.Any() && 
                m.Ratings.Average(r => r.Rating) >= options.MinRating.Value);

        if (options.MaxRating.HasValue)
            query = query.Where(m => m.Ratings.Any() && 
                m.Ratings.Average(r => r.Rating) <= options.MaxRating.Value);

        return query;
    }

    private static IQueryable<Movie> ApplyActorFilters(IQueryable<Movie> query, GetAllMoviesOptions options)
    {
        if (options.Actors?.Any() == true)
            query = query.Where(m => m.MovieActors
                .Any(ma => options.Actors.Contains(ma.ActorId)));

        return query;
    }

    private static IQueryable<Movie> ApplySorting(IQueryable<Movie> query, GetAllMoviesOptions options)
    {
        return options.SortField switch
        {
            "Title" => options.SortOrder == SortOrder.Ascending
                ? query.OrderBy(m => m.Title)
                : query.OrderByDescending(m => m.Title),
            "YearOfRelease" => options.SortOrder == SortOrder.Ascending
                ? query.OrderBy(m => m.YearOfRelease)
                : query.OrderByDescending(m => m.YearOfRelease),
            "Rating" => options.SortOrder == SortOrder.Ascending
                ? query.OrderBy(m => m.Ratings.Average(r => r.Rating))
                : query.OrderByDescending(m => m.Ratings.Average(r => r.Rating)),
            _ => query.OrderBy(m => m.Title)
        };
    }

    private static IQueryable<Movie> ApplyBasicFilters(IQueryable<Movie> query, string? title, int? yearOfRelease)
    {
        if (!string.IsNullOrEmpty(title))
            query = query.Where(m => m.Title.Contains(title));

        if (yearOfRelease.HasValue)
            query = query.Where(m => m.YearOfRelease == yearOfRelease.Value);

        return query;
    }

    private void UpdateMovieProperties(Movie existingMovie, Movie updatedMovie)
    {
        _context.Entry(existingMovie).CurrentValues.SetValues(updatedMovie);
        existingMovie.Genres = updatedMovie.Genres;
    }

    private async Task<bool> SaveChangesAsync(CancellationToken token) =>
        await _context.SaveChangesAsync(token) > 0;

    #endregion
}