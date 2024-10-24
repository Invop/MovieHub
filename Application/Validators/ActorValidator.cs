using System.Buffers.Text;
using FluentValidation;
using MovieHub.Application.Models;

namespace MovieHub.Application.Validators
{
    public class ActorValidator : AbstractValidator<Actor>
    {
        public ActorValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Actor name cannot be empty.")
                .MaximumLength(100).WithMessage("Actor name must not exceed 100 characters.");

            RuleFor(x => x.Biography)
                .MaximumLength(500).WithMessage("Biography must not exceed 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Biography));

            RuleFor(x => x.PhotoBase64)
                .Must(IsValidBase64).WithMessage("Photo must be valid.")
                .When(x => !string.IsNullOrEmpty(x.PhotoBase64));

            RuleFor(x => x.DateOfBirth)
                .LessThanOrEqualTo(DateTimeOffset.UtcNow).WithMessage("Date of Birth cannot be in the future.")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.PlaceOfBirth)
                .MaximumLength(100).WithMessage("Place of Birth must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.PlaceOfBirth));
        }

        private bool IsValidBase64(string? base64Text)
        {
            return Base64.IsValid(base64Text.AsSpan());
        }
    }
}