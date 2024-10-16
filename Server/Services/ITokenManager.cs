namespace MovieHub.Services;

public interface ITokenManager
{
    Task<string> GetTokenAsync();
}