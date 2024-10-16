using FluentValidation;
using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Models;

namespace MovieHub.Application.Validators;

public class GenreLookupValidator : AbstractValidator<GenreLookup>
{
    private readonly IGenreRepository _genreRepository;

    public GenreLookupValidator(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Genre name is required.")
            .MaximumLength(30)
            .WithMessage("Genre name must not exceed 30 characters.")
            .MustAsync(UniqueGenreName)
            .WithMessage("Genre name already exists.");
    }

    private async Task<bool> UniqueGenreName(string name, CancellationToken token)
    {
        var existingGenre = await _genreRepository.GetByNameAsync(name, token);
        return existingGenre == null;
    }
}