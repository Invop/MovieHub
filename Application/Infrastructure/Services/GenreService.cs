using FluentValidation;
using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Models;

namespace MovieHub.Application.Infrastructure.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepository _genreRepository;
    private readonly IValidator<GenreLookup> _genreValidator;

    public GenreService(IGenreRepository genreRepository, IValidator<GenreLookup> genreValidator)
    {
        _genreRepository = genreRepository;
        _genreValidator = genreValidator;
    }

    public async Task<IEnumerable<GenreLookup>> GetAllGenresAsync(CancellationToken token = default)
    {
        return await _genreRepository.GetAllAsync(token);
    }

    public async Task<GenreLookup?> GetGenreByIdAsync(int id, CancellationToken token = default)
    {
        return await _genreRepository.GetByIdAsync(id, token);
    }

    public Task<GenreLookup?> GetByNameAsync(string name, CancellationToken token)
    {
        return _genreRepository.GetByNameAsync(name, token);
    }

    public async Task<bool> CreateGenreAsync(GenreLookup genre, CancellationToken token = default)
    {
        await _genreValidator.ValidateAndThrowAsync(genre, token);
        return await _genreRepository.CreateAsync(genre, token);
    }

    public async Task<bool> UpdateGenreAsync(GenreLookup genre, CancellationToken token = default)
    {
        await _genreValidator.ValidateAndThrowAsync(genre, token);
        return await _genreRepository.UpdateAsync(genre, token);
    }

    public async Task<bool> DeleteGenreAsync(int id, CancellationToken token = default)
    {
        return await _genreRepository.DeleteAsync(id, token);
    }
}