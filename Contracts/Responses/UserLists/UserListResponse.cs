namespace MovieHub.Contracts.Responses.UserLists;

public class UserListResponse
{
    public Guid Id { get; init; }
    public Guid CreatedBy { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public bool IsPublic { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

}