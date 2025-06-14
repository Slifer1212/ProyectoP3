using Application.Dtos.AuthorDto;
using FluentValidation;

namespace Application.Validators.Format.AuthorValidations;

public class CreateAuthorFormatValidator : AbstractValidator<CreateAuthorDto>
{
    public CreateAuthorFormatValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.")
            .Matches(@"^[a-zA-ZÀ-ÿ\s'-]+$").WithMessage("First name contains invalid characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.")
            .Matches(@"^[a-zA-ZÀ-ÿ\s'-]+$").WithMessage("Last name contains invalid characters.");

        RuleFor(x => x.Biography)
            .MaximumLength(2000).WithMessage("Biography must not exceed 2000 characters.");

        RuleFor(x => x.BirthDate)
            .Must(BeAValidDate).WithMessage("Birth date must be a valid date.")
            .LessThan(DateTime.Now).WithMessage("Birth date cannot be in the future.");
        
        RuleFor(x => x.DeathDate)
            .Must(date => date == null || BeAValidDate(date.Value)).WithMessage("Death date must be a valid date.")
            .GreaterThan(x => x.BirthDate)
            .WithMessage("Death date must be after birth date.")
            .When(x => x.DeathDate != null);

        RuleFor(x => x.Nationality)
            .MaximumLength(50).WithMessage("Nationality must not exceed 50 characters.")
            .Matches(@"^[a-zA-ZÀ-ÿ\s'-]+$").WithMessage("Nationality contains invalid characters.")
            .When(x => !string.IsNullOrEmpty(x.Nationality));
    }

    private bool BeAValidDate(DateTime? date)
    {
        return date > new DateTime(1000, 1, 1) && date < DateTime.Now.AddYears(1);
    }
}