namespace MovieHub.Contracts.Requests;

public class CreateMovieRequest
{
    public required string Title { get; init; }

    public required int YearOfRelease { get; init; }

    public string Overview { get; init; }

    public string PosterBase64 { get; init; }

    public required IEnumerable<int> Genres { get; init; } = Enumerable.Empty<int>();
}