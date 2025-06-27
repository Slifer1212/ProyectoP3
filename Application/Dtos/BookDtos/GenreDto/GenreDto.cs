using Application.Dtos.BaseDto;

namespace Application.Dtos.GenreDto;

public class GenreDto : AuditEntityDto
{
    public Guid Id {get;  set;}
    public string? Name { get; set; }
    public string? Description { get; set; }
    
}