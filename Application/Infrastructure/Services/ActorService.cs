using FluentValidation;
using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Services;

public class ActorService: IActorService
{
    private readonly IActorRepository _actorRepository;
    private readonly IValidator<Actor> _actorValidator;
    private readonly IValidator<GetAllActorsOptions> _optionsValidator;

    public ActorService(IActorRepository actorRepository, IValidator<Actor> actorValidator, IValidator<GetAllActorsOptions> optionsValidator)
    {
        _actorRepository = actorRepository;
        _actorValidator = actorValidator;
        _optionsValidator = optionsValidator;
    }

    public Task<bool> CreateAsync(Actor actor, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Actor?> GetByIdAsync(Guid id, CancellationToken token = default)
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

    public Task<int> GetCountAsync(string? name, DateTimeOffset? dateOfBirth, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}