// FulfillReservationDto.cs
namespace Application.Dtos.LoansDtos.ReservationDto;

public class FulfillReservationDto
{
    public Guid Id { get; set; }
    public Guid LoanId { get; set; }
}