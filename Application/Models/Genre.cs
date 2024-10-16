namespace MovieHub.Application.Models;

public class Genre
{
    public Guid MovieId { get; set; }
    public int GenreId { get; set; }

    // Navigation properties
    public Movie Movie { get; set; } = default!;
    public GenreLookup GenreLookup { get; set; } = default!;
}