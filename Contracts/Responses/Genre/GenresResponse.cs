namespace MovieHub.Contracts.Responses.Genre;

public class GenresResponse
{
    public required IEnumerable<GenreResponse> Items { get; init; } = Enumerable.Empty<GenreResponse>();
}