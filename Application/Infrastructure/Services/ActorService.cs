using FluentValidation;
using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Services;

public class ActorService : IActorService
{
    private readonly IActorRepository _actorRepository;
    private readonly IValidator<Actor> _actorValidator;
    private readonly IValidator<GetAllActorsOptions> _optionsValidator;

    public ActorService(
        IActorRepository actorRepository, 
        IValidator<Actor> actorValidator,
        IValidator<GetAllActorsOptions> optionsValidator)
    {
        _actorRepository = actorRepository;
        _actorValidator = actorValidator;
        _optionsValidator = optionsValidator;
    }

    public async Task<bool> CreateAsync(Actor actor, CancellationToken token = default)
    {
        await _actorValidator.ValidateAndThrowAsync(actor, token);
        return await _actorRepository.CreateAsync(actor, token);
    }

    public Task<Actor?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return _actorRepository.GetByIdAsync(id, token);
    }

    public Task<Actor?> GetByNameAsync(string name, CancellationToken token = default)
    {
        return _actorRepository.GetByNameAsync(name, token);
    }

    public async Task<IEnumerable<Actor>> GetAllAsync(GetAllActorsOptions options, CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);
        return await _actorRepository.GetAllAsync(options, token);
    }

    public async Task<bool> UpdateAsync(Actor actor, CancellationToken token = default)
    {
        await _actorValidator.ValidateAndThrowAsync(actor, token);
        
        var actorExists = await _actorRepository.ExistsByIdAsync(actor.Id, token);
        if (!actorExists) return false;

        var success = await _actorRepository.UpdateAsync(actor, token);
        return success;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return _actorRepository.DeleteByIdAsync(id, token);
    }

    public Task<int> GetCountAsync(string? name, DateTimeOffset? dateOfBirth, CancellationToken token = default)
    {
        return _actorRepository.GetCountAsync(name, dateOfBirth, token);
    }
}