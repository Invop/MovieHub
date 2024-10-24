using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Services;

public class UserListService: IUserListService
{
    public Task<bool> CreateAsync(UserList userList, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserList?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserList>> GetAllAsync(GetAllUserLists getAllUserListsOptions, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(UserList userList, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetCountAsync(Guid? userId = default, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}