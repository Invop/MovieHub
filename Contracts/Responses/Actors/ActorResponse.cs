namespace MovieHub.Contracts.Responses.Actors;

public class ActorResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public string? PhotoBase64 { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
}