using Application.Interfaces.Repositories;
using Core.Books;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class GenreRepository : BaseRepository<Genre, Guid>, IGenreRepository
{
    public GenreRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<Genre?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(g => g.Name == name, cancellationToken);
    }
}