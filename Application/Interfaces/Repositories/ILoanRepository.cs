using Core.Loans;

namespace Application.Interfaces.Repositories;

public interface ILoanRepository : IBaseRepository<Loan, Guid>
{
    Task<IEnumerable<Loan>> GetActiveLoansByMemberAsync(Guid memberId);
    Task<IEnumerable<Loan>> GetOverdueLoansAsync();
    Task<Loan?> GetLoanByBookCopyAsync(Guid bookCopyId);
    Task<IEnumerable<Loan>> GetLoanHistoryByMemberAsync(Guid memberId);
    Task<int> GetActiveLoanCountByMemberAsync(Guid memberId);
}