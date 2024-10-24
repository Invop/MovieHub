namespace MovieHub.Application.Models;

public class Actor
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public string? PhotoBase64 { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    
    // Navigation properties
    public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
