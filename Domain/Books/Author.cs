using Core.BaseEntities;

namespace Core.Books;

public class Author : Person
{
    public Guid Id { get; private set; }
    public string Biography { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public DateTime? DeathDate { get; private set; }
    public string Nationality { get; private set; }
    
    private readonly List<Guid> _bookIds = new();
    public IReadOnlyList<Guid> BookIds => _bookIds.AsReadOnly();

    private Author() { }

    public static DomainResult<Author> Create(
        string firstName, string lastName,
         DateTime birthDate, DateTime deathDate,  string nationality, string biography = null)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(firstName))
            errors.Add("First name is required.");
        
        if (birthDate > DateTime.Now)
            errors.Add("Birth date cannot be in the future.");
        
        if (deathDate < birthDate)
            errors.Add("Death date cannot be before birth date.");

        if (string.IsNullOrWhiteSpace(nationality))
        {
            errors.Add("Nationality is required.");
        }
        
        if (errors.Any())
            return DomainResult<Author>.Failure(errors);

        var author = new Author
        {
            Id = Guid.NewGuid(),
            Name = firstName,
            LastName = lastName,
            Biography = biography,
            BirthDate = birthDate,
            DeathDate = deathDate,
            Nationality = nationality
        };
        return DomainResult<Author>.Success(author);
    }


    public DomainResult UpdateBiography(string biography)
    {
        if (string.IsNullOrWhiteSpace(biography))
            return DomainResult.Failure("Biography cannot be empty.");

        Biography = biography;
        return DomainResult.Success();
    }

    public DomainResult SetBirthDate(DateTime birthDate)
    {
        if (birthDate > DateTime.Now)
            return DomainResult.Failure("Birth date cannot be in the future.");

        BirthDate = birthDate;
        return DomainResult.Success();
    }

    public DomainResult AddBook(Guid bookId)
    {
        if (bookId == Guid.Empty)
            return DomainResult.Failure("Book ID cannot be empty.");

        if (_bookIds.Contains(bookId))
            return DomainResult.Failure("Book is already associated with this author.");

        _bookIds.Add(bookId);
        return DomainResult.Success();
    }

    public DomainResult RemoveBook(Guid bookId)
    {
        if (bookId == Guid.Empty)
            return DomainResult.Failure("Book ID cannot be empty.");

        if (!_bookIds.Contains(bookId))
            return DomainResult.Failure("Book is not associated with this author.");

        _bookIds.Remove(bookId);
        return DomainResult.Success();
    }
}