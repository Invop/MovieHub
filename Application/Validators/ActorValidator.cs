using System.Buffers.Text;
using FluentValidation;
using MovieHub.Application.Models;

namespace MovieHub.Application.Validators
{
    public class ActorValidator : AbstractValidator<Actor>
    {    
        private const int MinBirthYear = 1850;
        public ActorValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Actor name is required.")
                .MinimumLength(2)
                .WithMessage("Actor name must be at least 2 characters long.")
                .MaximumLength(100)
                .WithMessage("Actor name must not exceed 100 characters.")
                .Matches("^[a-zA-Z0-9 .',-]*$")
                .WithMessage("Actor name can only contain letters, numbers, spaces, and basic punctuation (.',-)");

            RuleFor(x => x.Biography)
                .MaximumLength(2000)
                .WithMessage("Biography must not exceed 2000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Biography));

            RuleFor(x => x.PhotoBase64)
                .Must(IsValidBase64).WithMessage("Photo must be valid.")
                .When(x => !string.IsNullOrEmpty(x.PhotoBase64));

            RuleFor(x => x.DateOfBirth)
                .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("Date of Birth cannot be in the future.")
                .GreaterThanOrEqualTo(DateTimeOffset.FromUnixTimeSeconds(0)
                    .AddYears(MinBirthYear - 1970))
                .WithMessage($"Date of Birth cannot be earlier than year {MinBirthYear}.")
                .Must(BeValidAge)
                .WithMessage("Actor must be at least 1 year old.")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.PlaceOfBirth)
                .MaximumLength(100).WithMessage("Place of Birth must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.PlaceOfBirth));
            
        }

        private bool IsValidBase64(string? base64Text)
        {
            return Base64.IsValid(base64Text.AsSpan());
        }
        private bool BeValidAge(DateTimeOffset? dateOfBirth)
        {
            if (!dateOfBirth.HasValue) return true;
        
            var age = DateTimeOffset.UtcNow.Year - dateOfBirth.Value.Year;
            if (dateOfBirth.Value.Date > DateTimeOffset.UtcNow.AddYears(-age)) age--;
        
            return age >= 1;
        }
        
    }
}