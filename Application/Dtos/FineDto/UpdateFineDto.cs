
namespace Application.Dtos.FineDto;

public class UpdateFineDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
}