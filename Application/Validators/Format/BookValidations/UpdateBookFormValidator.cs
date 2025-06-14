using Application.Dtos.BookDto;
using FluentValidation;

namespace Application.Validators.Format.BookValidations;

public class UpdateBookFormValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookFormValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Book ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MinimumLength(1).WithMessage("Title must have at least 1 character.");

        RuleFor(x => x.Isbn)
            .NotEmpty().WithMessage("ISBN is required.")
            .Matches(@"^\d{10}(\d{3})?$").WithMessage("ISBN must be 10 or 13 digits.");

        RuleFor(x => x.PublicationYear)
            .InclusiveBetween(1450, DateTime.Now.Year)
            .WithMessage($"Publication year must be between 1450 and {DateTime.Now.Year}.");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author ID is required.");

        RuleFor(x => x.GenreIds)
            .NotEmpty().WithMessage("At least one genre is required.")
            .Must(ids => ids.Count > 0).WithMessage("Genre IDs list cannot be empty.");

        RuleFor(x => x.Publisher)
            .MaximumLength(100).WithMessage("Publisher must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");;
    }
}