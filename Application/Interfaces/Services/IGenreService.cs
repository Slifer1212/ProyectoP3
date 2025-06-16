using Application.Dtos.BookDto;
using Application.Dtos.GenreDto;

namespace Application.Interfaces.Services;

public interface IGenreService
{
    Task<OperationResult<GenreDto>> GetByIdAsync(Guid id);
    Task<OperationResult<IEnumerable<GenreDto>>> GetAllAsync();
    Task<OperationResult<GenreDto>> CreateAsync(CreateGenreDto dto);
    Task<OperationResult<GenreDto>> UpdateAsync(Guid id, UpdateGenreDto dto);
    Task<OperationResult<bool>> DeleteAsync(Guid id);
    Task<OperationResult<IEnumerable<BookDto>>> GetBooksByGenreAsync(Guid genreId);    
}