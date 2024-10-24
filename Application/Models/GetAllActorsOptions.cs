namespace MovieHub.Application.Models;

public class GetAllActorsOptions
{
    public string? Name { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
}