using Application.Dtos;
using Application.Dtos.BookDto;

namespace Application.Interfaces.Services;

public interface IBookService
{
    Task<OperationResult<BookDto>> GetByIdAsync(Guid id);
    Task<OperationResult<IEnumerable<BookDto>>> GetAllAsync();
    Task<OperationResult<BookDto>> CreateAsync(CreateBookDto dto);
    Task<OperationResult<BookDto>> UpdateAsync(Guid id, UpdateBookDto dto);
    Task<OperationResult<bool>> DeleteAsync(Guid id);
}