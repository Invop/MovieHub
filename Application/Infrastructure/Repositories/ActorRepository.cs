using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MovieHub.Application.Data;
using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Repositories;

public class ActorRepository : IActorRepository
{
    private readonly MovieHubDbContext _context;

    public ActorRepository(MovieHubDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(Actor actor, CancellationToken token = default)
    {
        await _context.Actors.AddAsync(actor, token);
        return await SaveChangesAsync(token);
    }

    public async Task<Actor?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await GetActorByPredicate(a => a.Id == id, token);
    }

    public async Task<Actor?> GetByNameAsync(string name, CancellationToken token = default)
    {
        return await GetActorByPredicate(a => a.Name == name, token);
    }

    public async Task<IEnumerable<Actor>> GetAllAsync(GetAllActorsOptions options, CancellationToken token = default)
    {
        var query = BuildBaseActorQuery();
        query = ApplyFilters(query, options);
        query = ApplySorting(query, options);

        return await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync(token);
    }

    public async Task<bool> UpdateAsync(Actor actor, CancellationToken token = default)
    {
        var existingActor = await _context.Actors
            .Include(a => a.MovieActors)
            .FirstOrDefaultAsync(a => a.Id == actor.Id, token);

        if (existingActor == null) return false;

        UpdateActorProperties(existingActor, actor);
        return await SaveChangesAsync(token);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var actor = await _context.Actors.FindAsync([id], token);
        if (actor == null) return false;

        _context.Actors.Remove(actor);
        return await SaveChangesAsync(token);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default) =>
        await _context.Actors.AnyAsync(a => a.Id == id, token);

    public async Task<int> GetCountAsync(string? name, DateTimeOffset? dateOfBirth, CancellationToken token = default)
    {
        var query = _context.Actors.AsQueryable();
        query = ApplyBasicFilters(query, name, dateOfBirth);
        return await query.CountAsync(token);
    }

    #region Private Helper Methods

    private IQueryable<Actor> BuildBaseActorQuery() =>
        _context.Actors
            .Include(a => a.MovieActors)
            .ThenInclude(ma => ma.Movie)
            .AsSplitQuery();

    private async Task<Actor?> GetActorByPredicate(Expression<Func<Actor, bool>> predicate, CancellationToken token)
    {
        return await BuildBaseActorQuery()
            .FirstOrDefaultAsync(predicate, token);
    }

    private static IQueryable<Actor> ApplyFilters(IQueryable<Actor> query, GetAllActorsOptions options)
    {
        if (!string.IsNullOrEmpty(options.Name))
            query = query.Where(a => EF.Functions.Like(a.Name, $"%{options.Name}%"));

        if (options.DateOfBirth.HasValue)
            query = query.Where(a => a.DateOfBirth.HasValue && 
                a.DateOfBirth.Value.Date == options.DateOfBirth.Value.Date);

        if (!string.IsNullOrEmpty(options.PlaceOfBirth))
            query = query.Where(a => a.PlaceOfBirth != null && 
                EF.Functions.Like(a.PlaceOfBirth, $"%{options.PlaceOfBirth}%"));

        if (options.Movies?.Any() == true)
            query = query.Where(a => a.MovieActors
                .Any(ma => options.Movies.Contains(ma.MovieId)));

        return query;
    }

    private static IQueryable<Actor> ApplySorting(IQueryable<Actor> query, GetAllActorsOptions options)
    {
        return options.SortField switch
        {
            "Name" => options.SortOrder == SortOrder.Ascending
                ? query.OrderBy(a => a.Name)
                : query.OrderByDescending(a => a.Name),
            "DateOfBirth" => options.SortOrder == SortOrder.Ascending
                ? query.OrderBy(a => a.DateOfBirth)
                : query.OrderByDescending(a => a.DateOfBirth),
            "MovieCount" => options.SortOrder == SortOrder.Ascending
                ? query.OrderBy(a => a.MovieActors.Count)
                : query.OrderByDescending(a => a.MovieActors.Count),
            _ => query.OrderBy(a => a.Name)
        };
    }

    private static IQueryable<Actor> ApplyBasicFilters(IQueryable<Actor> query, string? name, DateTimeOffset? dateOfBirth)
    {
        if (!string.IsNullOrEmpty(name))
            query = query.Where(a => a.Name.Contains(name));

        if (dateOfBirth.HasValue)
            query = query.Where(a => a.DateOfBirth.HasValue && 
                a.DateOfBirth.Value.Date == dateOfBirth.Value.Date);

        return query;
    }

    private void UpdateActorProperties(Actor existingActor, Actor updatedActor)
    {
        _context.Entry(existingActor).CurrentValues.SetValues(updatedActor);
        existingActor.MovieActors = updatedActor.MovieActors;
    }

    private async Task<bool> SaveChangesAsync(CancellationToken token) =>
        await _context.SaveChangesAsync(token) > 0;

    #endregion
}