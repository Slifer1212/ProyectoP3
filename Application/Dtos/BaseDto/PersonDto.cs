namespace Application.Dtos.BaseDto;

public class PersonDto : AuditEntityDto
{
    public string? Name { get; set; }
    public string? LastName { get; set; }
}