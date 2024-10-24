namespace MovieHub.Application.Models;

public class MovieActor
{
    public Guid MovieId { get; set; }
    public Guid ActorId { get; set; }
    public string? Character { get; set; }
    
    // Navigation properties
    public Movie Movie { get; set; } = null!;
    public Actor Actor { get; set; } = null!;
}