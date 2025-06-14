using Core.Books;

namespace Application.Interfaces.Repositories;

public interface IAuthorRepository : IBaseRepository<Author, Guid>
{
    Task<Author?> GetByNameAsync(string name);
    Task<Author?> GetAuthorByNationalityAsync(string nationality);
}