
using Asp.Versioning;
using MovieHub.Api.Mapping;
using MovieHub.Application.Infrastructure.Services;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Logging;
using MovieHub.Application.Models;

namespace MovieHub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IGenreService _genreService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<MoviesController> _logger;
    private readonly IOutputCacheStore _outputCacheStore;

    public MoviesController(IMovieService movieService, IIdentityService identityService,
        ILogger<MoviesController> logger, IOutputCacheStore outputCacheStore, IGenreService genreService)
    {
        _movieService = movieService;
        _identityService = identityService;
        _logger = logger;
        _outputCacheStore = outputCacheStore;
        _genreService = genreService;
    }

    [Authorize(Roles = "admin,trusted_member")]
    [HttpPost(ApiEndpoints.Movies.Create)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request, CancellationToken token)
    {
        var movie = await request.MapToMovie(_genreService);
        await _movieService.CreateAsync(movie, token);
        await _outputCacheStore.EvictByTagAsync("movies", token);
        var movieResponse = movie.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movieResponse);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.Get)]
    [OutputCache(PolicyName = "MovieCache")]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug, CancellationToken token)
    {
        var userId = GetUserId();
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, token)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, token);
        if (movie is null) return NotFound();

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [AllowAnonymous]
    [OutputCache(PolicyName = "MovieCache")]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken token)
    {
        var userId = GetUserId();
        var options = request.MapToOptions().WithUser(userId);
        var movies = await _movieService.GetAllAsync(options, token);
        var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);
        var moviesResponse = movies.MapToResponse(request.Page, request.PageSize, movieCount);
        return Ok(moviesResponse);
    }

    [Authorize(Roles = "admin,trusted_member")]
    [HttpPut(ApiEndpoints.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieRequest request,
        CancellationToken token)
    {
        var movie = request.MapToMovie(id);
        var userId = GetUserId();
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, token);
        if (updatedMovie is null) return NotFound();

        await _outputCacheStore.EvictByTagAsync("movies", token);
        var response = updatedMovie.MapToResponse();
        return Ok(response);
    }

    [Authorize(Roles = "admin,trusted_member")]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken token)
    {
        var deleted = await _movieService.DeleteByIdAsync(id, token);
        if (!deleted) return NotFound();
        await _outputCacheStore.EvictByTagAsync("movies", token);
        return Ok();
    }

    private Guid? GetUserId()
    {
        var userIdStr = _identityService.GetUserIdentity();
        if (string.IsNullOrEmpty(userIdStr))
        {
            _logger.LogWarning("User identity is null or empty. Proceeding without user ID.");
            return null;
        }

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            _logger.LogError("User identity is not a valid GUID. Actual value: {UserIdStr}", userIdStr);
            throw new InvalidOperationException("User identity is not a valid GUID.");
        }

        return userId;
    }
}