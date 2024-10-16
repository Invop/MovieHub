namespace MovieHub.Contracts.Requests;

public class GetAllMoviesRequest : PagedRequest
{
    public string? Title { get; init; }

    public int? Year { get; init; }

    public IEnumerable<int>? GenreIds { get; init; }

    public string? SortBy { get; init; }
}