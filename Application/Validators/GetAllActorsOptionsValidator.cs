using FluentValidation;
using MovieHub.Application.Models;

namespace MovieHub.Application.Validators
{
    public class GetAllActorsOptionsValidator : AbstractValidator<GetAllActorsOptions>
    {
        private static readonly string[] AcceptableSortFields =
        [
            "name", "dateofbirth", "placeofbirth"
        ];

        public GetAllActorsOptionsValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .When(x => !string.IsNullOrEmpty(x.Name))
                .WithMessage("Name cannot be empty if provided");

            RuleFor(x => x.DateOfBirth)
                .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .When(x => x.DateOfBirth.HasValue)
                .WithMessage("Date of birth cannot be in the future");

            RuleFor(x => x.PlaceOfBirth)
                .NotEmpty()
                .When(x => !string.IsNullOrEmpty(x.PlaceOfBirth))
                .WithMessage("Place of birth cannot be empty if provided");

            RuleFor(x => x.SortField)
                .Must(x => x is null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
                .WithMessage("You can only sort by 'name' or 'dateofbirth' or 'placeofbirth'");

            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 25)
                .WithMessage("You can get between 1 and 25 actors per page");

            RuleFor(x => x.Movies)
                .Must(movies => movies == null || movies.All(movieId => movieId != Guid.Empty))
                .WithMessage("All movie IDs must be valid GUIDs");

            RuleFor(x => x.SortOrder)
                .IsInEnum()
                .WithMessage("Invalid sort order");
        }
    }
}