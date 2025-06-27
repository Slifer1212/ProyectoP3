// CreateReservationDto.cs
namespace Application.Dtos.LoansDtos.ReservationDto;

public class CreateReservationDto
{
    public Guid BookId { get; set; }
    public Guid MemberId { get; set; }
    public int ReservationDays { get; set; } = 7;
    public string? Notes { get; set; }
}