using Core.Enums.Loans;

namespace Application.Dtos.LoanDto;

public class CreateLoanDto
{
    public Guid MemberId { get;  set; }
    public Guid BookCopyId { get;  set; }
    public DateTime LoanDate { get;  set; }
    public DateTime DueDate { get;  set; }
    public DateTime? ReturnDate { get;  set; }
    public LoanStatus Status { get; set; } = LoanStatus.Active;
    public int RenewalCount { get;  set; } = 0;
    public int MaxRenewals { get;  set; } = 2;
    public string Notes { get;  set; }

    public Guid? FineId { get;  set; }
}