namespace MovieHub.Contracts.Requests.Actors;

public class GetAllActorsRequest : PagedRequest
{
    public string? Name { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}