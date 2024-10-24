using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Repositories;

public interface IUserListRepository
{
    Task<bool> CreateAsync(UserList userList, CancellationToken token = default);

    Task<UserList?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default);

    Task<IEnumerable<UserList>> GetAllAsync(GetAllUserLists getAllUserListsOptions,CancellationToken token = default);

    Task<bool> UpdateAsync(UserList userList, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);

    Task<int> GetCountAsync(Guid? userId = default, CancellationToken token = default);
}