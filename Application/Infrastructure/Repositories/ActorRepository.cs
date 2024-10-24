using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Repositories;

public class ActorRepository : IActorRepository
{
    public Task<bool> CreateAsync(Actor actor, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Actor?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Actor?> GetByNameAsync(string name, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Actor>> GetAllAsync(GetAllActorsOptions options, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Actor actor, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetCountAsync(string? name, DateTimeOffset? dateOfBirth, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}