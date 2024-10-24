
using Asp.Versioning;
using MovieHub.Api.Mapping;
using MovieHub.Application.Infrastructure.Services;
using MovieHub.Contracts.Requests;
using MovieHub.Contracts.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieHub.Api.Auth;
using MovieHub.Application.Models;
using MovieHub.Contracts.Requests.Genre;
using MovieHub.Contracts.Responses.Genre;

namespace MovieHub.Api.Controllers;

[ApiController]
[ApiVersion(1.0)]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly ILogger<GenresController> _logger;

    public GenresController(IGenreService genreService,
        ILogger<GenresController> logger)
    {
        _genreService = genreService;
        _logger = logger;
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Genres.Create)]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateGenreRequest request, CancellationToken token)
    {
        var result = await _genreService
            .CreateGenreAsync(new GenreLookup { Name = request.Name }, token);
        return result ? Ok() : NotFound();
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Genres.Get)]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] string idOrName, CancellationToken token)
    {
        GenreLookup? genre;
        if (int.TryParse(idOrName, out var id))
        {
            genre = await _genreService.GetGenreByIdAsync(id, token);
        }
        else
        {
            idOrName = idOrName.Trim().ToLower();
            genre = await _genreService.GetByNameAsync(idOrName, token);
        }

        if (genre is null) return NotFound();

        var response = genre.MapToResponse();
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Genres.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<GenreResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken token = default)
    {
        var genres = await _genreService.GetAllGenresAsync(token);
        return Ok(genres.MapToResponse());
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Genres.Update)]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateGenreRequest request,
        CancellationToken token)
    {
        var genre = new GenreLookup() { Id = id, Name = request.NewName };
        var result = await _genreService.UpdateGenreAsync(genre, token);
        return result ? Ok() : NotFound();
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Genres.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token)
    {
        var result = await _genreService.DeleteGenreAsync(id, token);
        return result ? Ok() : NotFound();
    }
}