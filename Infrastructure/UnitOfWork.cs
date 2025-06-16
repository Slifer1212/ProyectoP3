using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infraestructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infraestructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly LibraryDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IBookRepository? _bookRepository;
    private IAuthorRepository? _authorRepository;
    private IGenreRepository? _genreRepository;
    private IMemberRepository? _memberRepository;
    private ILoanRepository? _loanRepository;
    
    public UnitOfWork(LibraryDbContext context)
    {
        _context = context;
    }

    public IBookRepository Books => _bookRepository ??= new BookRepository(_context);
    public IAuthorRepository Authors => _authorRepository ??= new AuthorRepository(_context);
    public IGenreRepository Genres => _genreRepository ??= new GenreRepository(_context);
    
    public IMemberRepository Members => _memberRepository ??= new MemberRepository(_context);
    
    public ILoanRepository Loans => _loanRepository ??= new LoanRepository(_context);
    

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}