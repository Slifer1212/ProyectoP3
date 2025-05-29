namespace Core.Books;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public int PublicationYear { get; set; }
    public Author Author { get; set; }
    public List<Genre> Genre { get; set; }
    public string? Publisher { get; set; }
    public string? Despcription { get; set; }
}