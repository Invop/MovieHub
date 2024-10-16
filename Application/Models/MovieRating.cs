namespace MovieHub.Application.Models;

public class MovieRating
{
    public Guid UserId { get; set; }
    public Guid MovieId { get; set; }
    public int Rating { get; set; }

    // Navigation property
    public Movie Movie { get; set; } = default!;
}