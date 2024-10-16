namespace MovieHub.Contracts.Responses;

public class GenresResponse
{
    public required IEnumerable<GenreResponse> Items { get; init; } = Enumerable.Empty<GenreResponse>();
}