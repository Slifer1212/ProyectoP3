using Application.Dtos.GenreDto;
using Application.Interfaces;
using Application.Validators.Bussiness.Interfaces;

namespace Application.Validators.Bussiness.Implementations;

public class GenreBussinessValidator : IBussinessValidator<CreateGenreDto , UpdateGenreDto, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenreBussinessValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateCreateAsync(CreateGenreDto dto)
    {
        var errors = new List<string>();

        // Check if genre already exists with the same name
        var existingGenre = _unitOfWork.Genres.FindSingleAsync(g => g.Name == dto.Name);
        if (existingGenre != null)
        {
            errors.Add("A genre with this name already exists.");
        }

        var existDescription = _unitOfWork.Genres.FindSingleAsync(g => g.Description == dto.Description);
        if (existDescription != null)
        {
            errors.Add("A genre with this description already exists.");
        }
        
        return (errors.Count == 0, errors);
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateUpdateAsync(UpdateGenreDto dto)
    {
        var errors = new List<string>();
        
        // Check if genre exists
        var existingGenre = await _unitOfWork.Genres.GetByIdAsync(dto.Id);
        if (existingGenre == null)
        {
            errors.Add("Genre not found.");
            return (false, errors);
        }

        // Check if name conflicts with another genre
        if (dto.Name != existingGenre.Name)
        {
            var genreWithSameName = await _unitOfWork.Genres.FindSingleAsync(g => 
                g.Name == dto.Name && g.Id != dto.Id);
            if (genreWithSameName != null)
            {
                errors.Add("Another genre with this name already exists.");
            }
        }
        
        // Check if description conflicts with another genre
        if (dto.Description != existingGenre.Description)
        {
            var genreWithSameDescription = await _unitOfWork.Genres.FindSingleAsync(g => 
                g.Description == dto.Description && g.Id != dto.Id);
            if (genreWithSameDescription != null)
            {
                errors.Add("Another genre with this description already exists.");
            }
        }

        return (errors.Count == 0, errors);
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateDeleteAsync(Guid id)
    {
        var errors = new List<string>();
        
        // Check if genre exists
        var existingGenre = _unitOfWork.Genres.GetByIdAsync(id);
        if (existingGenre == null)
        {
            errors.Add("Genre not found.");
            return (false, errors);
        }
        
        // Check if genre is associated with any books
        var booksWithGenre = _unitOfWork.Books.FindAsync(b => b.GenreIds.Contains(id));
        if (booksWithGenre != null)
        {
            errors.Add("Cannot delete genre because it is associated with one or more books.");
            return (false, errors);
        }
        
        return (errors.Count == 0, errors);
    }
}