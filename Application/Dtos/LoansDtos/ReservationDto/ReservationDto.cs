using Application.Dtos.BaseDto;
using Core.Enums.Loans;

namespace Application.Dtos.LoansDtos.ReservationDto;

public class ReservationDto : AuditEntityDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid MemberId { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public ReservationStatus Status { get; set; }
    public int QueuePosition { get; set; }
    public DateTime? NotifiedDate { get; set; }
    public DateTime? ReadyDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}