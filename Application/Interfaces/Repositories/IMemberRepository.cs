using Core.Enums.User;
using Core.Users;

namespace Application.Interfaces.Repositories;
public interface IMemberRepository : IBaseRepository<Member, Guid>
{
    Task<Member?> GetByIdentityUserIdAsync(Guid identityUserId);
    Task<Member?> GetByEmailAsync(string email);
    Task<IEnumerable<Member>> GetMembersWithOverdueLoansAsync();
    Task<Member?> GetMemberWithLoansAsync(Guid memberId);
    Task<IEnumerable<Member>> GetExpiredMembershipsAsync();
    Task<IEnumerable<Member>> GetMembersWithOutstandingFinesAsync();
    Task<IEnumerable<Member>> GetMembersByTypeAsync(MemberShipType membershipType);
    Task<IEnumerable<Member>> GetMembersWithExpiringMembershipAsync(int daysBeforeExpiry);
    Task<IEnumerable<Member>> SearchMembersAsync(string searchTerm);
    Task<object> GetMembershipStatisticsAsync();
}