using Application.Dtos.AuthorDto;
using Application.Interfaces;
using Application.Validators.Bussiness.Interfaces;

namespace Application.Validators.Bussiness.Implementations;

public class AuthorBussinessValidator : IBussinessValidator<CreateAuthorDto, UpdateAuthorDto, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthorBussinessValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateCreateAsync(CreateAuthorDto dto)
    {
        var errors = new List<string>();

        // Check if author already exists with the same name
        var existingAuthor = await _unitOfWork.Authors.FindSingleAsync(a => 
            a.Name == dto.Name && a.LastName == dto.LastName);
            
        if (existingAuthor != null)
        {
            errors.Add("An author with this name already exists.");
        }

        // Validate birth and death dates
        if (dto.BirthDate.HasValue && dto.DeathDate.HasValue && dto.DeathDate < dto.BirthDate)
        {
            errors.Add("Death date cannot be earlier than birth date.");
        }

        return (errors.Count == 0, errors);
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateUpdateAsync(UpdateAuthorDto dto)
    {
        var errors = new List<string>();

        // Check if author exists
        var existingAuthor = await _unitOfWork.Authors.GetByIdAsync(dto.Id);
        if (existingAuthor == null)
        {
            errors.Add("Author not found.");
            return (false, errors);
        }

        // Check if name conflicts with another author
        if (dto.Name != existingAuthor.Name || dto.LastName != existingAuthor.LastName)
        {
            var authorWithSameName = await _unitOfWork.Authors.FindSingleAsync(a => 
                a.Name == dto.Name && 
                a.LastName == dto.LastName && 
                a.Id != dto.Id);
                
            if (authorWithSameName != null)
            {
                errors.Add("Another author with this name already exists.");
            }
        }

        // Validate birth and death dates
        if (dto.BirthDate.HasValue && dto.DeathDate.HasValue && dto.DeathDate < dto.BirthDate)
        {
            errors.Add("Death date cannot be earlier than birth date.");
        }

        return (errors.Count == 0, errors);
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateDeleteAsync(Guid id)
    {
        var errors = new List<string>();

        // Verify if the author exists
        var author = await _unitOfWork.Authors.GetByIdAsync(id);
        if (author == null)
        {
            errors.Add("The specified author does not exist.");
            return (false, errors);
        }

        // Check if the author has books associated
        var authorBooks = await _unitOfWork.Books.FindAsync(b => b.Author.Id == id);
        if (authorBooks.Any())
        {
            errors.Add("This author cannot be deleted because they have books in the library.");
        }

        return (errors.Count == 0, errors);
    }
}