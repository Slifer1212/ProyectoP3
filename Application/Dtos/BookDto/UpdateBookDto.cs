namespace Application.Dtos.BookDto;

public class UpdateBookDto : CreateBookDto
{
    public Guid Id { get; set; }
}