using Application.Dtos.BookDto;
using FluentValidation;

namespace Application.Validators;

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");
        
        RuleFor(x => x.Isbn).NotEmpty().WithMessage("ISBN is required.")
            .MaximumLength(13).WithMessage("ISBN must not exceed 13 characters.");
        
        RuleFor(x => x.PublicationYear).InclusiveBetween(0, DateTime.Now.Year)
            .WithMessage($"Publication year must be between 0 and {DateTime.Now.Year}.");
        
        RuleFor(x => x.AuthorId).NotEmpty().WithMessage("Author ID is required.");
        
        RuleFor(x => x.GenreIds).NotEmpty().WithMessage("At least one genre ID is required.")
            .Must(ids => ids.Count > 0).WithMessage("Genre IDs must not be empty.");
        
        
        RuleFor(x => x.Publisher).MaximumLength(100)
            .WithMessage("Publisher must not exceed 100 characters.");
        
        RuleFor(x => x.Description).MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
        
    }
}