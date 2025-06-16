using Core.Users;

namespace Application.Interfaces.Repositories;

public interface ILibrarianRepository : IBaseRepository<Librarian, Guid>
{
    Task<Librarian?> GetByIdentityUserIdAsync(Guid identityUserId);
    Task<Librarian?> GetByEmailAsync(string email);
    Task<IEnumerable<Librarian>> SearchLibrariansAsync(string searchTerm);
    Task<IEnumerable<Librarian>> GetLibrariansWithActiveMembershipsAsync();
    Task<object> GetLibrarianStatisticsAsync();
}