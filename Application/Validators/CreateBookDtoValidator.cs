using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Isbn)
            .NotEmpty().WithMessage("ISBN is required")
            .Matches(@"^\d{13}$").WithMessage("ISBN must be 13 digits");

        RuleFor(x => x.PublicationYear)
            .InclusiveBetween(1450, DateTime.Now.Year)
            .WithMessage("Invalid publication year");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author is required");

        RuleFor(x => x.GenreIds)
            .NotEmpty().WithMessage("At least one genre is required");
    }
}