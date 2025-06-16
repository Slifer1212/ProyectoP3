using Core.BaseEntities;

namespace Core.Books;

public class Book : AuditEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public int PublicationYear { get; set; }
    public Author Author { get; set; }
    public List<Genre> Genre { get; set; }
    public string? Publisher { get; set; }
    public string? Description { get; set; }
    
    private Book() { }
    
    private readonly List<Guid> _genreIds = new();
    public IReadOnlyList<Guid> GenreIds => _genreIds.AsReadOnly();
    
    private readonly List<Guid> _copyIds = new();
    public IReadOnlyList<Guid> CopyIds => _copyIds.AsReadOnly();

    public static DomainResult<Book> Create(
        string title, string isbn, int publicationYear, Guid authorId
         , Author author, Genre genre ,  string? publisher = null, string? description = null)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(title))
            errors.Add("Title is required.");
        
        if (string.IsNullOrWhiteSpace(isbn))
            errors.Add("ISBN is required.");
        
        if (publicationYear < 1450 || publicationYear > DateTime.Now.Year)
            errors.Add("Publication year is out of range.");
        
        if (authorId == Guid.Empty)
            errors.Add("Author is required.");
        
        if (author == null)
            errors.Add("The book must have an author.");
        
        if (genre == null)
            errors.Add("The book must have at least one genre.");
        
        if (string.IsNullOrWhiteSpace(publisher))
            errors.Add("Publisher is required.");
        
        if (description != null && description.Length > 500)
            errors.Add("Description cannot exceed 500 characters.");
        
        
        if (errors.Any())
            return DomainResult<Book>.Failure(errors);

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Isbn = isbn,
            PublicationYear = publicationYear,
            Author = author,
            Publisher = publisher,
            Description = description

        };
        return DomainResult<Book>.Success(book);
    }

    public DomainResult UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return DomainResult.Failure("title cannot be empty.");

        Title = title.Trim();
        return DomainResult.Success();
    }

    public DomainResult UpdateDescription(string description)
    {
        if (description != null && description.Length > 500)
            return DomainResult.Failure("Description cannot exceed 500 characters.");

        Description = description?.Trim();
        return DomainResult.Success();
    }

    public DomainResult AddGenre(Guid genreId)
    {
        if (genreId == Guid.Empty)
            return DomainResult.Failure("Genre ID cannot be empty.");

        if (_genreIds.Contains(genreId))
            return DomainResult.Failure("Genre already exists in the book.");

        _genreIds.Add(genreId);
        return DomainResult.Success();
    }

    public DomainResult RemoveGenre(Guid genreId)
    {
        if (genreId == Guid.Empty)
            return DomainResult.Failure("Genre ID cannot be empty.");

        if (!_genreIds.Contains(genreId))
            return DomainResult.Failure("Genre does not exist in the book.");

        _genreIds.Remove(genreId);
        return DomainResult.Success();
    }
    

    public DomainResult Deactivate()
    {
        if (IsDeleted ==  true)
            return DomainResult.Failure("Book is already deactivated.");

        IsDeleted = true;
        return DomainResult.Success();
    }
    public int TotalCopies => _copyIds.Count;
}