
using Core.BaseEntities;
using Core.Enums;
using Core.Enums.Loans;

namespace Core.Loans;

public class Loan : AuditEntity
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }
    public Guid BookCopyId { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; }
    public int RenewalCount { get; private set; } = 0;
    public int MaxRenewals { get; private set; } = 2;
    public string Notes { get; private set; }

    public Guid? FineId { get; private set; }

    private Loan()
    {
    }

    public static DomainResult<Loan> Create(Guid memberId, Guid bookCopyId, int loanDurationDays = 14)
    {
        var errors =  new List<string>();
        if (memberId == Guid.Empty)
           errors.Add("Member ID cannot be empty.");

        if (bookCopyId == Guid.Empty)
            errors.Add("Book copy ID cannot be empty.");

        if (loanDurationDays <= 0)
            errors.Add("Loan duration must be greater than zero days.");

        if (errors.Any())
            return DomainResult<Loan>.Failure(errors);

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            BookCopyId = bookCopyId,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(loanDurationDays),
            Status = LoanStatus.Active
        };

        return DomainResult<Loan>.Success(loan);
    }

    public bool IsOverdue()
    {
        return DueDate < DateTime.UtcNow && ReturnDate == null;
    }

    public bool CanBeRenewed()
    {
        return RenewalCount < MaxRenewals &&
               Status == LoanStatus.Active &&
               !IsOverdue() &&
               FineId == null;
    }

    public bool IsActive()
    {
        return Status == LoanStatus.Active && ReturnDate == null;
    }

    public int DaysOverdue()
    {
        if (!IsOverdue() && ReturnDate == null)
            return 0;

        var referenceDate = ReturnDate ?? DateTime.UtcNow;
        return Math.Max(0, (int)(referenceDate - DueDate).TotalDays);
    }

    public decimal CalculateOverdueFine(decimal dailyFineRate)
    {
        if (dailyFineRate <= 0)
            return 0;

        return DaysOverdue() * dailyFineRate;
    }

    public DomainResult Return()
    {
        if (ReturnDate != null)
            return DomainResult.Failure("Loan has already been returned.");

        ReturnDate = DateTime.UtcNow;
        Status = ReturnDate > DueDate ? LoanStatus.ReturnedLate : LoanStatus.ReturnedOnTime;

        return DomainResult.Success();
    }

    public DomainResult Renew(int additionalDays = 14)
    {
        if (!CanBeRenewed())
            return DomainResult.Failure("Loan cannot be renewed.");

        if (additionalDays <= 0)
            return DomainResult.Failure("Additional days must be greater than zero.");

        DueDate = DueDate.AddDays(additionalDays);
        RenewalCount++;

        return DomainResult.Success();
    }

    public DomainResult MarkOverdue()
    {
        if (!IsOverdue())
            return DomainResult.Failure("Loan is not overdue.");

        if (Status == LoanStatus.Overdue)
            return DomainResult.Failure("Loan is already marked as overdue.");

        Status = LoanStatus.Overdue;

        return DomainResult.Success();
    }

    public DomainResult AddFine(Guid fineId)
    {
        if (fineId == Guid.Empty)
            return DomainResult.Failure("Fine ID cannot be empty.");

        if (FineId != null)
            return DomainResult.Failure("Fine already associated with this loan.");

        FineId = fineId;

        return DomainResult.Success();
    }

    public DomainResult AddNotes(string notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
            return DomainResult.Failure("Notes cannot be empty.");

        Notes = string.IsNullOrEmpty(Notes) ? notes : $"{Notes}\n{notes}";

        return DomainResult.Success();
    }
}