using Application.Dtos.BaseDto;

namespace Application.Dtos.BookDto;

public class BookDto : AuditEntityDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public int PublicationYear { get; set; }
    public AuthorDto.AuthorDto Author { get; set; }
    public List<GenreDto.GenreDto> Genres { get; set; } 
    public string? Publisher { get; set; }
    public string? Description { get; set; }
    public int TotalCopies { get; set; } 
}