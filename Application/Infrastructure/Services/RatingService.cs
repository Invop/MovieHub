using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Services;

public class RatingService : IRatingService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IRatingRepository _ratingRepository;

    public RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository)
    {
        _ratingRepository = ratingRepository;
        _movieRepository = movieRepository;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default)
    {
        if (rating is < 0 or > 5) return false;

        var movieExists = await _movieRepository.ExistsByIdAsync(movieId, token);
        if (!movieExists) return false;

        return await _ratingRepository.RateMovieAsync(movieId, rating, userId, token);
    }

    public Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        return _ratingRepository.DeleteRatingAsync(movieId, userId, token);
    }

    public Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        return _ratingRepository.GetRatingsForUserAsync(userId, token);
    }
}