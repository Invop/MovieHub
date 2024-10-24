using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MovieHub.Application.Infrastructure.Services;

namespace MovieHub.Api.Controllers;
[ApiController]
[ApiVersion(1.0)]
public class ActorsController
{
    private readonly IActorService _actorService;
    private readonly ILogger<MoviesController> _logger;
    private readonly IOutputCacheStore _outputCacheStore;

    public ActorsController(
        ILogger<MoviesController> logger, IOutputCacheStore outputCacheStore,IActorService actorService)
    {
        _logger = logger;
        _outputCacheStore = outputCacheStore;
        _actorService = actorService;
    }
}