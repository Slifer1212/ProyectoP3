namespace Application.Dtos.BookDto;

public class UpdateBookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public int PublicationYear { get; set; }
    public Guid AuthorId { get; set; }
    public List<Guid> GenreIds { get; set; }
    public string? Publisher { get; set; }
    public string? Description { get; set; }
}