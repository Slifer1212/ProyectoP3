using Core.Enums.Loans;

namespace Application.Dtos.FineDto;

public class CreateFineDto
{
    public Guid MemberId { get; set; }
    public Guid? LoanId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public FineType Type { get; set; }
}