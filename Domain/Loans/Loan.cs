using Core.BaseEntities;
using Core.Enums;

namespace Core.Loans;

public class Loan : AuditEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; private set; }
    public Guid BookId { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public LoanStatus Status { get; private set; }
}