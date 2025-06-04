// Domain/Loans/Fine.cs

using Core.BaseEntities;
using Core.Enums;
using Core.Enums.Loans;

namespace Core.Loans;

public class Fine : AuditEntity
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }
    public Guid? LoanId { get; private set; } // Puede ser null para multas no relacionadas con préstamos
    public decimal Amount { get; private set; }
    public string Reason { get; private set; }
    public FineType Type { get; private set; }
    public bool IsPaid { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public decimal PaidAmount { get; private set; }

    public decimal RemainingAmount => Amount - PaidAmount;
    public PaymentMethods PaymentMethod { get; private set; }

    private Fine()
    {
    }
    
    public static DomainResult<Fine> Create(Guid memberId, decimal amount, string reason, FineType type,
        Guid? loanId = null)
    {
        var errors = new List<string>();

        if (memberId == Guid.Empty)
            errors.Add("Please provide a valid member ID.");

        if (amount <= 0)
            errors.Add("Please provide a valid amount.");

        if (string.IsNullOrWhiteSpace(reason))
            errors.Add("Please provide a valid reason.");

        if (errors.Any())
            return DomainResult<Fine>.Failure(errors);

        var fine = new Fine
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            Amount = amount,
            Reason = reason,
            Type = type,
            LoanId = loanId,
            IsPaid = false,
            PaidAmount = 0
        };
        return DomainResult<Fine>.Success(fine);
    }

    public bool IsOverdue()
    {
        // Consider a fine overdue if it's not paid and was created more than 30 days ago
        return !IsPaid && CreatedAt.AddDays(30) < DateTime.UtcNow;
    }

    public DomainResult MarkAsPaid(PaymentMethods paymentMethod, string paymentReference)
    {
        if (IsPaid)
            return DomainResult.Failure("Fine is already paid.");

        if (PaidAmount > 0 && PaidAmount < Amount)
            return DomainResult.Failure("Fine has partial payments. Use PartialPayment method to complete payment.");

        IsPaid = true;
        PaidAt = DateTime.UtcNow;
        PaidAmount = Amount;
        PaymentMethod = paymentMethod;

        return DomainResult.Success();
    }

    public DomainResult PartialPayment(decimal amount, PaymentMethods paymentMethod, string paymentReference)
    {
        if (IsPaid)
            return DomainResult.Failure("Fine is already paid.");

        if (amount <= 0)
            return DomainResult.Failure("Payment amount must be greater than zero.");

        if (amount > RemainingAmount)
            return DomainResult.Failure("Payment amount exceeds remaining balance.");

        PaidAmount += amount;
        PaymentMethod = paymentMethod;

        if (PaidAmount >= Amount)
        {
            IsPaid = true;
            PaidAt = DateTime.UtcNow;
        }

        return DomainResult.Success();
    }

    public DomainResult Waive(string reason)
    {
        if (IsPaid)
            return DomainResult.Failure("Fine is already paid.");

        if (string.IsNullOrWhiteSpace(reason))
            return DomainResult.Failure("Waiver reason cannot be empty.");

        IsPaid = true;
        PaidAt = DateTime.UtcNow;
        Reason += $" [WAIVED: {reason}]";

        return DomainResult.Success();
    }

    public DomainResult UpdateAmount(decimal newAmount)
    {
        if (IsPaid)
            return DomainResult.Failure("Cannot update amount of a paid fine.");

        if (newAmount <= 0)
            return DomainResult.Failure("Fine amount must be greater than zero.");

        if (newAmount < PaidAmount)
            return DomainResult.Failure("New amount cannot be less than already paid amount.");

        Amount = newAmount;

        return DomainResult.Success();
    }
}