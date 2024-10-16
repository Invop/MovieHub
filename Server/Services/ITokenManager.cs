namespace MovieHub.Services;

public interface ITokenManager
{
    Task<string> GetTokenAsync(HttpRequestMessage request, CancellationToken cancellationToken);
}