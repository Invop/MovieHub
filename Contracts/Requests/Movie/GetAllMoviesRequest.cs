namespace MovieHub.Contracts.Requests.Movie;

public class GetAllMoviesRequest : PagedRequest
{
    public string? Title { get; init; }

    public int? Year { get; init; }

    public int? MinRating { get; init; }
    public int? MaxRating { get; init; }
    public IEnumerable<int>? GenreIds { get; init; }

    public string? SortBy { get; init; }
}