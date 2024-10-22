using FluentValidation;
using MovieHub.Application.Models;

namespace MovieHub.Application.Validators;

public class GetAllMoviesOptionsValidator : AbstractValidator<GetAllMoviesOptions>
{
    private static readonly string[] AcceptableSortFields =
    {
        "title", "yearofrelease","rating"
    };

    public GetAllMoviesOptionsValidator()
    {
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year)
            .When(x => x.YearOfRelease.HasValue)
            .WithMessage("Year of release cannot be in the future");

        RuleFor(x => x.SortField)
            .Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage("You can only sort by 'title' or 'yearofrelease' or 'rating'");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 25)
            .WithMessage("You can get between 1 and 25 movies per page");

        RuleFor(x => x.Genres)
            .Must(genres => genres == null || genres.All(genre => genre > 0))
            .WithMessage("All genre IDs must be positive");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .When(x => x.UserId.HasValue)
            .WithMessage("UserId, if provided, cannot be empty");

        RuleFor(x => x.SortOrder)
            .IsInEnum()
            .WithMessage("Invalid sort order");
    }
}