namespace Application.Dtos.GenreDto;

public class UpdateGenreDto
{
    public Guid Id {get;  set;}
    public string? Name { get; set; }
    public string? Description { get; set; }
    
}