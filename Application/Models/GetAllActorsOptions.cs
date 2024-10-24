namespace MovieHub.Application.Models;

public class GetAllActorsOptions
{
    public string? Name { get; set; }

    public DateTimeOffset? DateOfBirth { get; set; }

    public string? PlaceOfBirth { get; set; }

    public IEnumerable<Guid>? Movies { get; set; }

    public string? SortField { get; set; }

    public SortOrder? SortOrder { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}
