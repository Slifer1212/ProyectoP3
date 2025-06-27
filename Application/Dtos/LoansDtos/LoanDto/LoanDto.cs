using Application.Dtos.BaseDto;
using Core.Enums.Loans;

namespace Application.Dtos.LoansDtos.LoanDto;

public class LoanDto : AuditEntityDto
{
    public Guid Id { get;  set; }
    public Guid MemberId { get;  set; }
    public Guid BookCopyId { get;  set; }
    public DateTime LoanDate { get;  set; }
    public DateTime DueDate { get;  set; }
    public DateTime? ReturnDate { get;  set; }
    public LoanStatus Status { get;  set; }
    public int RenewalCount { get;  set; } = 0;
    public int MaxRenewals { get;  set; } = 2;
    public string Notes { get;  set; }

    public Guid? FineId { get;  set; }
}