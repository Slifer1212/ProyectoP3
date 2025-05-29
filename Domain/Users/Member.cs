using Core.BaseEntities;
using Core.Enums;
using Core.Loans;

namespace Core.Users;

public class Member : BaseUser
{
    List<Loan> Loans { get; set; }
    List<Reservation> Reservations { get; set; }
    
    public override string Role { get; } = "Member";
    public DateTime MembershipExpiry { get; set; }
    
    public MemberShipType MemberShipType { get; set; }
    
    public override bool CanAccessResource(string resource)
    {
        return MembershipExpiry > DateTime.Now;
    }
}