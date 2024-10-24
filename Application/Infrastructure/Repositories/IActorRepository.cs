using MovieHub.Application.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MovieHub.Application.Infrastructure.Repositories
{
    public interface IActorRepository
    {
        Task<bool> CreateAsync(Actor actor, CancellationToken token = default);

        Task<Actor?> GetByIdAsync(Guid id, CancellationToken token = default);

        Task<Actor?> GetByNameAsync(string name, CancellationToken token = default);
        
        Task<IEnumerable<Actor>> GetAllAsync(GetAllActorsOptions options, CancellationToken token = default);

        Task<bool> UpdateAsync(Actor actor, CancellationToken token = default);

        Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

        Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);

        Task<int> GetCountAsync(string? name, DateTimeOffset? dateOfBirth, CancellationToken token = default);
    }
}