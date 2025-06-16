using Application.Interfaces.Repositories;
using Core.Enums.Loans;
using Core.Loans;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class LoanRepository : BaseRepository<Loan, Guid> , ILoanRepository
{
    public LoanRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Loan>> GetActiveLoansByMemberAsync(Guid memberId)
    {
        return await _dbSet
            .Where(l => l.MemberId == memberId && 
                        l.Status == LoanStatus.Active &&
                        l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();    
    }

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync()
    {
        var today = DateTime.UtcNow.Date;
        
        return await _dbSet
            .Where(l => l.DueDate < today && 
                        l.ReturnDate == null &&
                        (l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue))
            .OrderBy(l => l.DueDate)
            .ToListAsync();
    }

    public async Task<Loan?> GetLoanByBookCopyAsync(Guid bookCopyId)
    {
        return await _dbSet
            .Where(l => l.BookCopyId == bookCopyId && 
                        l.ReturnDate == null &&
                        (l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue))
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Loan>> GetLoanHistoryByMemberAsync(Guid memberId)
    {
        return await _dbSet
            .Where(l => l.MemberId == memberId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync();    }

    public async Task<int> GetActiveLoanCountByMemberAsync(Guid memberId)
    {
        return await _dbSet
            .CountAsync(l => l.MemberId == memberId && 
                             l.Status == LoanStatus.Active &&
                             l.ReturnDate == null);    
    }
}