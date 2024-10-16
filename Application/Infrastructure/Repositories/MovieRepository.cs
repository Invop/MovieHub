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
        var result = await _context.SaveChangesAsync(token);
        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default,
        CancellationToken token = default)
    {
        var movie = await _context.Movies
            .Include(m => m.Genres)
            .ThenInclude(g => g.GenreLookup)
            .Include(m => m.Ratings)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id, token);

        if (movie == null) return null;

        movie.Rating = movie.Ratings.Any() ? MathF.Round(movie.Ratings.Average(r => (float)r.Rating), 1) : null;
        movie.UserRating =
            userId.HasValue ? movie.Ratings.FirstOrDefault(r => r.UserId == userId.Value)?.Rating : null;
        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default,
        CancellationToken token = default)
    {
        var movie = await _context.Movies
            .Include(m => m.Genres)
            .ThenInclude(g => g.GenreLookup)
            .Include(m => m.Ratings)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Slug == slug, token);

        if (movie == null) return null;

        movie.Rating = movie.Ratings.Any() ? MathF.Round(movie.Ratings.Average(r => (float)r.Rating), 1) : null;
        movie.UserRating =
            userId.HasValue ? movie.Ratings.FirstOrDefault(r => r.UserId == userId.Value)?.Rating : null;
        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options,
        CancellationToken token = default)
    {
        var query = _context.Movies
            .Include(m => m.Genres)
            .ThenInclude(g => g.GenreLookup)
            .Include(m => m.Ratings)
            .AsQueryable()
            .AsSplitQuery();

        if (!string.IsNullOrEmpty(options.Title)) query = query.Where(m => m.Title.Contains(options.Title));

        if (options.YearOfRelease.HasValue) query = query.Where(m => m.YearOfRelease == options.YearOfRelease.Value);

        if (options.Genres != null && options.Genres.Any())
            query = query.Where(m => m.Genres.Any(g => options.Genres.Contains(g.GenreId)));

        switch (options.SortField)
        {
            case "Title":
                query = options.SortOrder == SortOrder.Ascending
                    ? query.OrderBy(m => m.Title)
                    : query.OrderByDescending(m => m.Title);
                break;
            case "YearOfRelease":
                query = options.SortOrder == SortOrder.Ascending
                    ? query.OrderBy(m => m.YearOfRelease)
                    : query.OrderByDescending(m => m.YearOfRelease);
                break;
            default:
                query = query.OrderBy(m => m.Title);
                break;
        }

        var movieList = await query.Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync(token);

        return movieList.Select(m =>
        {
            m.Rating = m.Ratings.Any() ? MathF.Round(m.Ratings.Average(r => (float)r.Rating), 1) : null;
            m.UserRating = options.UserId.HasValue
                ? m.Ratings.FirstOrDefault(r => r.UserId == options.UserId.Value)?.Rating
                : null;
            return m;
        });
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        var existingMovie = await _context.Movies
            .Include(m => m.Genres)
            .ThenInclude(g => g.GenreLookup)
            .FirstOrDefaultAsync(m => m.Id == movie.Id, token);
        if (existingMovie == null) return false;

        _context.Entry(existingMovie).CurrentValues.SetValues(movie);
        existingMovie.Genres = movie.Genres;
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var movie = await _context.Movies.FindAsync(new object[] { id }, token);
        if (movie == null) return false;

        _context.Movies.Remove(movie);
        var result = await _context.SaveChangesAsync(token);
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _context.Movies.AnyAsync(m => m.Id == id, token);
    }

    public async Task<int> GetCountAsync(string? title, int? yearOfRelease,
        CancellationToken token = default)
    {
        var query = _context.Movies.AsQueryable();

        if (!string.IsNullOrEmpty(title)) query = query.Where(m => m.Title.Contains(title));

        if (yearOfRelease.HasValue) query = query.Where(m => m.YearOfRelease == yearOfRelease.Value);

        return await query.CountAsync(token);
    }
}