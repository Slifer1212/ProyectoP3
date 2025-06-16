using Application.Dtos.GenreDto;
using FluentValidation;

namespace Application.Validators.Format.GenreValidations;

public class CreateGenreValidatior : AbstractValidator<CreateGenreDto>
{
    public CreateGenreValidatior()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")            
            .Matches(@"^[a-zA-ZÀ-ÿ\s'-]+$").WithMessage("Name contains invalid characters.");
        
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}