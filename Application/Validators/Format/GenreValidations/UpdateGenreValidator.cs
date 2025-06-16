using Application.Dtos.GenreDto;
using FluentValidation;

namespace Application.Validators.Format.GenreValidations;

public class UpdateGenreValidator : AbstractValidator<UpdateGenreDto>
{
    public UpdateGenreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters.")
            .Matches(@"^[a-zA-ZÀ-ÿ\s'-]+$").WithMessage("Name contains invalid characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}