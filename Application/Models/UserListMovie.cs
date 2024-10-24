namespace MovieHub.Application.Models;

public class UserListMovie
{
    public Guid ListId { get; set; }
    public Guid MovieId { get; set; }
    public DateTimeOffset AddedAt { get; set; }
    
    // Navigation properties
    public UserList UserList { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}
