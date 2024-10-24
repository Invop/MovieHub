using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieHub.Application.Data;
using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Infrastructure.Services;

namespace MovieHub.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<MovieHubDbContext>(options =>
            options.UseNpgsql(connectionString));
            
        return services;
    }
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IRatingRepository, RatingRepository>();
        services.AddScoped<IRatingService, RatingService>();
        services.AddScoped<IActorRepository, ActorRepository>();
        services.AddScoped<IActorService, ActorService>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IMovieService, MovieService>();
        services.AddScoped<IUserListRepository, UserListRepository>();
        services.AddScoped<IUserListService, UserListService>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();
        return services;
    }
    
}