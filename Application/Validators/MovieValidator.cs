using System.Buffers.Text;
using FluentValidation;
using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Models;

namespace MovieHub.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMovieRepository _movieRepository;

    public MovieValidator(IMovieRepository movieRepository, IGenreRepository genreRepository)
    {
        _movieRepository = movieRepository;
        _genreRepository = genreRepository;

        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Genres)
            .NotEmpty()
            .MustAsync(GenresMustExist)
            .WithMessage("Invalid genres");

        RuleForEach(x => x.Genres)
            .ChildRules(genres =>
            {
                genres.RuleFor(g => g.GenreLookup.Name)
                    .NotEmpty()
                    .WithMessage("Genre name is required and cannot be empty.");
            }).WithMessage("Invalid genres");
        ;

        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists in the system");
        
        /*RuleFor(x => x.PosterBase64)
            .Must(IsValidBase64)
            .WithMessage("Poster must be a valid Base64 string.");*/
    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token = default)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug, token: token);

        if (existingMovie != null && existingMovie.Id != movie.Id) return false;

        return true;
    }

    private bool IsValidBase64(string? base64Text)
    {
        if (string.IsNullOrEmpty(base64Text)) return true; // Allow null values

        return Base64.IsValid(base64Text.AsSpan());
    }

    private async Task<bool> GenresMustExist(ICollection<Genre> genres, CancellationToken token)
    {
        foreach (var genre in genres)
        {
            var existingGenre = await _genreRepository.GetByIdAsync(genre.GenreId, token);
            if (existingGenre == null) return false;
        }

        return true;
    }
}