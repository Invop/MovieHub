using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Services;

public interface IGenreService
{
    Task<IEnumerable<GenreLookup>> GetAllGenresAsync(CancellationToken token = default);
    Task<GenreLookup?> GetGenreByIdAsync(int id, CancellationToken token = default);
    Task<GenreLookup?> GetByNameAsync(string name, CancellationToken token);
    Task<bool> CreateGenreAsync(GenreLookup genre, CancellationToken token = default);
    Task<bool> UpdateGenreAsync(GenreLookup genre, CancellationToken token = default);
    Task<bool> DeleteGenreAsync(int id, CancellationToken token = default);
}