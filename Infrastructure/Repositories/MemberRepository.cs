using Application;
using Application.Interfaces.Repositories;
using Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories;

public class MemberRepository : BaseRepository<Member, Guid>, IMemberRepository
{
    public MemberRepository(LibraryDbContext context) : base(context)
    {
    }

    public async Task<Member?> GetByIdentityUserIdAsync(Guid identityUserId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId);
    }

    public async Task<Member?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.Email == email);
    }

    public async Task<IEnumerable<Member>> GetMembersWithOverdueLoansAsync()
    {
        var today = DateTime.UtcNow.Date;

        var membersWithOverdueLoans = await (
            from member in _dbSet
            join loan in _context.Loans on member.Id equals loan.MemberId
            where loan.DueDate < today &&
                  loan.ReturnDate == null &&
                  loan.Status == Core.Enums.Loans.LoanStatus.Overdue
            select member
        ).Distinct().ToListAsync();

        return membersWithOverdueLoans;
    }

    public async Task<Member?> GetMemberWithLoansAsync(Guid memberId)
    {
        // Get the member
        var member = await _dbSet.FirstOrDefaultAsync(m => m.Id == memberId);

        if (member == null)
            return null;

        var activeLoanIds = member.ActiveLoanIds.ToList();

        if (activeLoanIds.Any())
        {
            var loans = await _context.Loans
                .Where(l => activeLoanIds.Contains(l.Id))
                .ToListAsync();

        }

        return member;
    }

    public async Task<IEnumerable<Member>> GetExpiredMembershipsAsync()
    {
        var today = DateTime.UtcNow.Date;

        return await _dbSet
            .Where(m => m.MembershipExpiry < today &&
                       m.IsActive == true &&
                       m.MemberShipState != Core.Enums.User.MemberShipState.Expired)
            .ToListAsync();
    }

    /// <summary>
    /// Gets members with outstanding fines
    /// </summary>
    public async Task<IEnumerable<Member>> GetMembersWithOutstandingFinesAsync()
    {
        return await _dbSet
            .Where(m => m.OutstandingFines > 0 && m.IsActive == true)
            .OrderByDescending(m => m.OutstandingFines)
            .ToListAsync();
    }

    /// <summary>
    /// Gets members by membership type
    /// </summary>
    public async Task<IEnumerable<Member>> GetMembersByTypeAsync(Core.Enums.User.MemberShipType membershipType)
    {
        return await _dbSet
            .Where(m => m.MemberShipType == membershipType &&
                       m.IsActive == true)
            .ToListAsync();
    }

    /// <summary>
    /// Gets members whose membership expires in the coming days
    /// </summary>
    public async Task<IEnumerable<Member>> GetMembersWithExpiringMembershipAsync(int daysBeforeExpiry)
    {
        var today = DateTime.UtcNow.Date;
        var expiryDate = today.AddDays(daysBeforeExpiry);

        return await _dbSet
            .Where(m => m.MembershipExpiry >= today &&
                       m.MembershipExpiry <= expiryDate &&
                       m.IsActive == true)
            .OrderBy(m => m.MembershipExpiry)
            .ToListAsync();
    }

    /// <summary>
    /// Searches for members by name or email
    /// </summary>
    public async Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm)
    {
        var normalizedSearchTerm = searchTerm.ToLower();

        return await _dbSet
            .Where(m => m.Name!.ToLower().Contains(normalizedSearchTerm) ||
                       m.LastName!.ToLower().Contains(normalizedSearchTerm) ||
                       m.Email.ToLower().Contains(normalizedSearchTerm) ||
                       (m.Name + " " + m.LastName).ToLower().Contains(normalizedSearchTerm) ||
                       m.PhoneNumber.ToLower().Contains(normalizedSearchTerm))
            .ToListAsync();
    }

    /// <summary>
    /// Gets membership statistics
    /// </summary>
    public async Task<object> GetMembershipStatisticsAsync()
    {
        var stats = await _dbSet
            .GroupBy(m => m.MemberShipType)
            .Select(g => new
            {
                MembershipType = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        var totalMembers = await _dbSet.CountAsync();
        var activeMembers = await _dbSet.CountAsync(m => m.IsActive == true);
        var membersWithFines = await _dbSet.CountAsync(m => m.OutstandingFines > 0);
        var totalFines = await _dbSet.SumAsync(m => m.OutstandingFines);

        return new
        {
            TotalMembers = totalMembers,
            ActiveMembers = activeMembers,
            MembersWithFines = membersWithFines,
            TotalOutstandingFines = totalFines,
            MembershipTypeDistribution = stats
        };
    }
}