using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Services;

public interface IActorService
{
    Task<bool> CreateAsync(Actor actor, CancellationToken token = default);

    Task<Actor?> GetByIdAsync(Guid id, CancellationToken token = default);

    Task<IEnumerable<Actor>> GetAllAsync(GetAllActorsOptions options, CancellationToken token = default);

    Task<bool> UpdateAsync(Actor actor, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<int> GetCountAsync(string? name, DateTimeOffset? dateOfBirth, CancellationToken token = default);
}