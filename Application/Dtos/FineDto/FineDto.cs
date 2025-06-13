using Application.Dtos.BaseDto;
using Core.Enums.Loans;

namespace Application.Dtos.FineDto;

public class FineDto : AuditEntityDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public Guid? LoanId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public FineType Type { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount => Amount - PaidAmount;
    public PaymentMethods PaymentMethod { get; set; }
}