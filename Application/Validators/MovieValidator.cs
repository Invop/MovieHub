using System.Buffers.Text;
using FluentValidation;
using MovieHub.Application.Infrastructure.Repositories;
using MovieHub.Application.Models;

namespace MovieHub.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IGenreRepository _genreRepository;
    private readonly IMovieRepository _movieRepository;
    private const int MinYear = 1888;
    private const int MaxGenres = 5;
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 2000;

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

        RuleFor(x => x.Genres)
            .NotEmpty()
            .WithMessage("At least one genre is required.")
            .Must(genres => genres.Count <= MaxGenres)
            .WithMessage($"Movie cannot have more than {MaxGenres} genres.")
            .MustAsync(GenresMustExist)
            .WithMessage("One or more genres are invalid.");
        ;


        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Movie title is required.")
            .MinimumLength(1)
            .WithMessage("Movie title must not be empty.")
            .MaximumLength(MaxTitleLength)
            .WithMessage($"Movie title must not exceed {MaxTitleLength} characters.")
            .Must(BeValidTitle)
            .WithMessage("Movie title can only contain letters, numbers, spaces, and basic punctuation (.,':-!)");


        RuleFor(x => x.YearOfRelease)
            .NotEmpty()
            .WithMessage("Release year is required.")
            .GreaterThanOrEqualTo(MinYear)
            .WithMessage($"Release year cannot be earlier than {MinYear}.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 5)
            .WithMessage("Release year cannot be more than 5 years in the future.");

        RuleFor(x => x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists in the system");

        RuleFor(x => x.PosterBase64)
            .Must(IsValidBase64)
            .WithMessage("Poster must be valid")
            .When(x => !string.IsNullOrEmpty(x.PosterBase64));
        
        RuleFor(x => x.Overview)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage($"Description must not exceed {MaxDescriptionLength} characters.")
            .When(x => !string.IsNullOrEmpty(x.Overview));
    }

    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token = default)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug, token: token);

        if (existingMovie != null && existingMovie.Id != movie.Id) return false;

        return true;
    }

    private bool IsValidBase64(string? base64Text)
    {
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

    private bool BeValidTitle(string title)
    {
        return !string.IsNullOrEmpty(title) &&
               title.All(c => char.IsLetterOrDigit(c) || " .,':-!".Contains(c));
    }
}