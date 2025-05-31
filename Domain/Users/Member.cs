using Core.BaseEntities;
using Core.Enums;
using Core.Enums.User;

namespace Core.Users;

public class Member : LibraryUser
{
    public DateTime MembershipExpiry { get; private set; }
    public MemberShipType MemberShipType { get; private set; }
    public DateTime MembershipStartDate { get; private set; }
    public decimal OutstandingFines { get; private set; }
    public bool IsActive { get; private set; } = true;

    public MemberShipState MemberShipState { get; private set; } = MemberShipState.Active;

    private readonly List<Guid> _activeLoanIds = new();
    private readonly List<Guid> _activeReservationIds = new();
    private readonly List<Guid> _fineIds = new();

    public IReadOnlyList<Guid> ActiveLoanIds => _activeLoanIds.AsReadOnly();
    public IReadOnlyList<Guid> ActiveReservationIds => _activeReservationIds.AsReadOnly();
    public IReadOnlyList<Guid> FineIds => _fineIds.AsReadOnly();

    public override string Role { get; } = "Member";

    public int MaxLoansAllowed => MemberShipType switch
    {
        MemberShipType.Basic => 3,
        MemberShipType.Premium => 5,
        MemberShipType.Vip => 10,
        _ => 1
    };

    public int MaxReservationsAllowed => MemberShipType switch
    {
        MemberShipType.Basic => 2,
        MemberShipType.Premium => 4,
        MemberShipType.Vip => 8,
        _ => 1
    };

    private Member()
    {
    }

    public static DomainResult<Member> Create(Guid identityUserId, string firstName, string lastName, string email,
        MemberShipType membershipType, DateTime membershipStartDate, DateTime membershipExpiry)
    {
        var errors = new List<string>();

        if (membershipStartDate >= membershipExpiry)
            errors.Add("Membership start date must be before the expiry date.");

        if (membershipType == MemberShipType.None)
            errors.Add("Membership type is required.");

        if (errors.Any())
            return DomainResult<Member>.Failure(errors);

        var member = new Member
        {
            Id = Guid.NewGuid(),
            IdentityUserId = identityUserId,
            Name = firstName,
            LastName = lastName,
            Email = email,
            MemberShipType = membershipType,
            MembershipStartDate = membershipStartDate,
            MembershipExpiry = membershipExpiry
        };

        return DomainResult<Member>.Success(member);
    }

    public bool IsMembershipActive()
    {
        if (!IsActive || MemberShipState != MemberShipState.Active)
            return false;

        if (MembershipExpiry < DateTime.UtcNow)
            return false;

        return true;
    }


    public bool CanBorrowBooks()
    {
        if (!IsActive || MemberShipState != MemberShipState.Active)
            return false;

        if (MembershipExpiry < DateTime.UtcNow)
            return false;

        return _activeLoanIds.Count < MaxLoansAllowed;
    }

    public bool CanMakeReservations()
    {
        if (!IsActive || MemberShipState != MemberShipState.Active)
            return false;

        if (MembershipExpiry < DateTime.UtcNow)
            return false;

        return _activeReservationIds.Count < MaxReservationsAllowed;
    }

    public bool HasOverdueFines()
    {
        return OutstandingFines > 0;
    }

    public override bool CanAccessResource(string resource)
    {
        return resource.StartsWith("Book") ||
               resource.StartsWith("Catalog") ||
               resource.StartsWith("Reservation") ||
               resource.StartsWith("MyAccount");
    }

    public DomainResult ExtendMembership(DateTime newExpiry)
    {
        if (newExpiry <= MembershipExpiry)
            return DomainResult.Failure("New expiry date must be after current expiry date.");

        MembershipExpiry = newExpiry;
        return DomainResult.Success();
    }

    public DomainResult UpgradeMembership(MemberShipType newType)
    {
        if (newType <= MemberShipType.None || newType == MemberShipType.Basic)
            return DomainResult.Failure("Invalid membership type for upgrade.");

        MemberShipType = newType;
        return DomainResult.Success();
    }

    public DomainResult AddActiveLoan(Guid loanId)
    {
        if (_activeLoanIds.Count >= MaxLoansAllowed)
            return DomainResult.Failure("Maximum active loans limit reached.");

        if (_activeLoanIds.Contains(loanId))
            return DomainResult.Failure("Loan already exists.");

        _activeLoanIds.Add(loanId);
        return DomainResult.Success();
    }

    public DomainResult RemoveActiveLoan(Guid loanId)
    {
        if (!_activeLoanIds.Contains(loanId))
            return DomainResult.Failure("Loan not found in active loans.");

        _activeLoanIds.Remove(loanId);
        return DomainResult.Success();
    }

    public DomainResult AddActiveReservation(Guid reservationId)
    {
        if (_activeReservationIds.Count >= MaxReservationsAllowed)
            return DomainResult.Failure("Maximum active reservations limit reached.");

        if (_activeReservationIds.Contains(reservationId))
            return DomainResult.Failure("Reservation already exists.");

        _activeReservationIds.Add(reservationId);
        return DomainResult.Success();
    }

    public DomainResult RemoveActiveReservation(Guid reservationId)
    {
        if (!_activeReservationIds.Contains(reservationId))
            return DomainResult.Failure("Reservation not found in active reservations.");

        _activeReservationIds.Remove(reservationId);
        return DomainResult.Success();
    }

    public DomainResult AddLoan(Guid loanId)
    {
        if (!IsMembershipActive())
            return DomainResult.Failure("Membership is not active.");

        if (_activeLoanIds.Count >= MaxLoansAllowed)
            return DomainResult.Failure("Maximum active loans limit reached.");

        if (_activeLoanIds.Contains(loanId))
            return DomainResult.Failure("Loan already exists.");

        _activeLoanIds.Add(loanId);
        return DomainResult.Success();
    }

    public DomainResult CompleteLoan(Guid loanId)
    {
        if (!_activeLoanIds.Contains(loanId))
            return DomainResult.Failure("Loan not found in active loans.");

        _activeLoanIds.Remove(loanId);
        return DomainResult.Success();
    }

    public DomainResult AddReservation(Guid reservationId)
    {
        if (!IsMembershipActive())
            return DomainResult.Failure("Membership is not active.");

        if (_activeReservationIds.Count >= MaxReservationsAllowed)
            return DomainResult.Failure("Maximum active reservations limit reached.");

        if (_activeReservationIds.Contains(reservationId))
            return DomainResult.Failure("Reservation already exists.");

        _activeReservationIds.Add(reservationId);
        return DomainResult.Success();
    }

    public DomainResult CompleteReservation(Guid reservationId)
    {
        if (!_activeReservationIds.Contains(reservationId))
            return DomainResult.Failure("Reservation not found in active reservations.");

        _activeReservationIds.Remove(reservationId);
        return DomainResult.Success();
    }

    public DomainResult AddFine(Guid fineId, decimal amount)
    {
        if (amount <= 0)
            return DomainResult.Failure("Fine amount must be greater than zero.");

        if (_fineIds.Contains(fineId))
            return DomainResult.Failure("Fine already exists.");

        _fineIds.Add(fineId);
        OutstandingFines += amount;
        return DomainResult.Success();
    }

    public DomainResult PayFine(Guid fineId, decimal amount)
    {
        if (!_fineIds.Contains(fineId))
            return DomainResult.Failure("Fine not found.");

        if (amount <= 0)
            return DomainResult.Failure("Payment amount must be greater than zero.");

        if (amount > OutstandingFines)
            return DomainResult.Failure("Payment amount exceeds outstanding fines.");

        OutstandingFines -= amount;

        // If all fines are paid, we can remove this fine ID
        if (OutstandingFines == 0)
        {
            _fineIds.Remove(fineId);
        }

        return DomainResult.Success();
    }

    public DomainResult Suspend()
    {
        if (!IsActive)
            return DomainResult.Failure("Member is already suspended.");

        IsActive = false;
        MemberShipState = MemberShipState.Suspended;
        return DomainResult.Success();
    }

    public DomainResult Reactivate()
    {
        if (IsActive)
            return DomainResult.Failure("Member is already active.");

        IsActive = true;
        MemberShipState = MemberShipState.Active;
        return DomainResult.Success();
    }
}