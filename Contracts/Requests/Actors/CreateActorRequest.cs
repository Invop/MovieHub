namespace MovieHub.Contracts.Requests.Actors;

public class CreateActorRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public string? PhotoBase64 { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
}