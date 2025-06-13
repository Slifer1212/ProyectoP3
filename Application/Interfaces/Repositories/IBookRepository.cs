using Core.Books;

namespace Application.Interfaces.Repositories;

public interface IBookRepository : IBaseRepository<Book , Guid>
{
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<IEnumerable<Book>> GetByAuthorAsync(Guid authorId);
    Task<IEnumerable<Book>> GetByGenreAsync(Guid genreId);
    Task<IEnumerable<Book>> GetAvailableBooks();
    Task<Book?> GetBookWithCopiesAsync(Guid bookId);
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
}