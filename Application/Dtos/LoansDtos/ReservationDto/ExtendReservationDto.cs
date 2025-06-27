using Core.Enums.Loans;

namespace Application.Dtos.LoansDtos.ReservationDto;

public class ExtendReservationDto
{
    public Guid Id { get; set; }
    public int AdditionalDays { get; set; }
}