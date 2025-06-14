using Application.Dtos.AuthorDto;
using Application.Dtos.BookDto;

namespace Application.Interfaces.Services;

public interface IAuthorService 
{
    Task<OperationResult<AuthorDto>> GetByIdAsync(Guid id);
    Task<OperationResult<IEnumerable<AuthorDto>>> GetAllAsync();
    Task<OperationResult<AuthorDto>> CreateAsync(CreateAuthorDto dto);
    Task<OperationResult<AuthorDto>> UpdateAsync(Guid id, UpdateAuthorDto dto);
    Task<OperationResult<bool>> DeleteAsync(Guid id);
    Task<OperationResult<IEnumerable<BookDto>>> GetBooksByAuthorAsync(Guid authorId);
}