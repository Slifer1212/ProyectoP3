using Application.Dtos.BaseDto;

namespace Application.Dtos.AuthorDto;

public class AuthorDto : PersonDto
{
    public Guid Id { get; set; }
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? Nationality { get; set; }
}