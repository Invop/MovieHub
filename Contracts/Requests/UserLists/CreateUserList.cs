namespace MovieHub.Contracts.Requests.UserLists;

public class CreateUserList
{
    public string Name { get; init; }
    public string? Description { get; init; }
    public bool IsPublic { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

}