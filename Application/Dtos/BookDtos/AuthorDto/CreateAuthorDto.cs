using Application.Dtos.BaseDto;

namespace Application.Dtos.AuthorDto;

public class CreateAuthorDto : PersonDto
{
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? Nationality { get; set; }
}