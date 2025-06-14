using Application.Interfaces.Repositories;
using Core.Books;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class AuthorRepository : BaseRepository<Author, Guid>, IAuthorRepository
{
    public AuthorRepository(LibraryDbContext context) : base(context)
    {
    }
    
    public async Task<Author?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.Name == name || 
                                      (a.Name + " " + a.LastName) == name);
    }

    public async Task<Author?> GetAuthorByNationalityAsync(string nationality)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.Nationality == nationality);
    }
}