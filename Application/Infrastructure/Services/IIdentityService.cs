namespace MovieHub.Application.Infrastructure.Services;

public interface IIdentityService
{
    string GetUserIdentity();

    string GetUserName();
}