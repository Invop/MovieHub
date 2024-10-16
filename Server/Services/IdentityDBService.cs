using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MovieHub.Server.Data;
using Radzen;

namespace MovieHub.Server.Services;

public class IdentityDbService
{
    private readonly NavigationManager _navigationManager;

    public IdentityDbService(IdentityDbContext context, NavigationManager navigationManager)
    {
        Context = context;
        this._navigationManager = navigationManager;
    }

    private IdentityDbContext Context { get; }

    public void Reset()
    {
        Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList()
            .ForEach(e => e.State = EntityState.Detached);
    }

    public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
    {
        if (query != null)
        {
            if (!string.IsNullOrEmpty(query.Filter))
            {
                if (query.FilterParameters != null)
                    items = items.Where(query.Filter, query.FilterParameters);
                else
                    items = items.Where(query.Filter);
            }

            if (!string.IsNullOrEmpty(query.OrderBy)) items = items.OrderBy(query.OrderBy);

            if (query.Skip.HasValue) items = items.Skip(query.Skip.Value);

            if (query.Top.HasValue) items = items.Take(query.Top.Value);
        }
    }
}