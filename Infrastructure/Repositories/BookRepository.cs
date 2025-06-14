using Application.Interfaces.Repositories;
using Core.Books;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class BookRepository : BaseRepository<Book, Guid>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.Isbn == isbn);
    }

    public async Task<IEnumerable<Book>> GetByAuthorAsync(Guid authorId)
    {
        return await _dbSet
            .Where(b => b.Author.Id == authorId)
            .Include(b => b.Author)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetByGenreAsync(Guid genreId)
    {
        return await _dbSet
            .Where(b => b.GenreIds.Contains(genreId))
            .Include(b => b.Author)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetAvailableBooks()
    {
        // Obtener libros que tienen al menos una copia disponible
        var availableBookIds = await _context.Copies
            .Where(c => c.Status == Core.Enums.Book.BookCopyStatus.Available)
            .Select(c => c.BookId)
            .Distinct()
            .ToListAsync();

        return await _dbSet
            .Where(b => availableBookIds.Contains(b.Id))
            .Include(b => b.Author)
            .ToListAsync();
    }

    public async Task<Book?> GetBookWithCopiesAsync(Guid bookId)
    {
        return await _dbSet
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == bookId);
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
    {
        var normalizedSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(b => b.Title.ToLower().Contains(normalizedSearchTerm) ||
                       b.Isbn.ToLower().Contains(normalizedSearchTerm) ||
                       b.Author.Name!.ToLower().Contains(normalizedSearchTerm) ||
                       b.Author.LastName!.ToLower().Contains(normalizedSearchTerm) ||
                       (b.Description != null && b.Description.ToLower().Contains(normalizedSearchTerm)))
            .Include(b => b.Author)
            .ToListAsync();
    }
}