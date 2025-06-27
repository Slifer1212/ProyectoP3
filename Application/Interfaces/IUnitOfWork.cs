using Application.Interfaces.Repositories;

namespace Application.Interfaces;


public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }
    IAuthorRepository Authors { get; }
    IGenreRepository Genres { get; }
    IMemberRepository Members { get; }
    ILoanRepository Loans { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}