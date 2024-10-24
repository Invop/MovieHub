namespace MovieHub.Application.Models;

public class GetAllUserLists
{
    public Guid? UserId { get; set; }
    public UserListType? UserListType { get; set; } = Models.UserListType.PrivateAndPublic;
}

public enum UserListType
{
    Private,
    Public,
    PrivateAndPublic
}