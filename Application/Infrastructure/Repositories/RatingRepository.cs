using Microsoft.EntityFrameworkCore;
using MovieHub.Application.Data;
using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly MovieHubDbContext _context;

    public RatingRepository(MovieHubDbContext context)
    {
        _context = context;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
    {
        var existingRating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId, token);

        if (existingRating != null)
        {
            existingRating.Rating = rating;
        }
        else
        {
            var newRating = new MovieRating { MovieId = movieId, UserId = userId, Rating = rating };
            await _context.Ratings.AddAsync(newRating, token);
        }

        var result = await _context.SaveChangesAsync(token);
        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        var rating = await _context.Ratings
            .Where(r => r.MovieId == movieId)
            .AverageAsync(r => (float?)r.Rating, token);

        return rating.HasValue ? MathF.Round(rating.Value, 1) : null;
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId,
        CancellationToken token = default)
    {
        var ratingAvg = await _context.Ratings
            .Where(r => r.MovieId == movieId)
            .AverageAsync(r => (float?)r.Rating, token);

        var userRating = await _context.Ratings
            .Where(r => r.MovieId == movieId && r.UserId == userId)
            .Select(r => (int?)r.Rating)
            .FirstOrDefaultAsync(token);

        return (ratingAvg.HasValue ? MathF.Round(ratingAvg.Value, 1) : null, userRating);
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        var rating = await _context.Ratings
            .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId, token);

        if (rating == null)
            return false;

        _context.Ratings.Remove(rating);
        var result = await _context.SaveChangesAsync(token);
        return result > 0;
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        return await _context.Ratings
            .Where(r => r.UserId == userId)
            .Include(r => r.Movie)
            .ToListAsync(token);
    }
}