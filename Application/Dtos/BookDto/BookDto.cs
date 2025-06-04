namespace Application.Dtos.BookDto;

public class BookDto
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
    public int AvailableCopies { get; set; }
}