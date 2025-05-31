// Domain/Users/UserPreferences.cs

using Core.BaseEntities;
using Core.Record;

namespace Core.Users;

public class UserPreferences : AuditEntity
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }

    // Preferencias de géneros y autores
    private readonly List<Guid> _favoriteGenreIds = new();
    private readonly List<Guid> _favoriteAuthorIds = new();

    public IReadOnlyList<Guid> FavoriteGenreIds => _favoriteGenreIds.AsReadOnly();
    public IReadOnlyList<Guid> FavoriteAuthorIds => _favoriteAuthorIds.AsReadOnly();

    // Configuración de notificaciones
    public bool EmailNotifications { get; private set; } = true;
    public bool SmsNotifications { get; private set; } = false;
    public bool PushNotifications { get; private set; } = true;
    public bool LoanReminders { get; private set; } = true;
    public bool RecommendationAlerts { get; private set; } = true;
    public bool ReservationNotifications { get; private set; } = true;

    // Preferencias de lectura
    public string PreferredLanguage { get; private set; }
    public ReadingGoal? CurrentReadingGoal { get; private set; }

    private UserPreferences()
    {
    }

    // Factory method y métodos de dominio - tú implementas
    public static DomainResult<UserPreferences> Create(Guid memberId)
    {
        var errors = new List<string>();
        
        if (memberId == Guid.Empty)
            errors.Add("Member ID cannot be empty.");
        
        if (errors.Any())
            return DomainResult<UserPreferences>.Failure(errors);

        var preferences = new UserPreferences
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            PreferredLanguage = "English" // Default language
        };

        return DomainResult<UserPreferences>.Success(preferences);
    }

    public DomainResult AddFavoriteGenre(Guid genreId)
    {
        if (genreId == Guid.Empty)
            return DomainResult.Failure("Genre ID cannot be empty.");

        if (_favoriteGenreIds.Contains(genreId))
            return DomainResult.Failure("Genre is already in favorites.");

        if (_favoriteGenreIds.Count >= 20) // Reasonable limit
            return DomainResult.Failure("Maximum number of favorite genres reached.");

        _favoriteGenreIds.Add(genreId);
        return DomainResult.Success();
    }

    public DomainResult RemoveFavoriteGenre(Guid genreId)
    {
        if (!_favoriteGenreIds.Contains(genreId))
            return DomainResult.Failure("Genre not found in favorites.");

        _favoriteGenreIds.Remove(genreId);
        return DomainResult.Success();
    }

    public DomainResult AddFavoriteAuthor(Guid authorId)
    {
        if (authorId == Guid.Empty)
            return DomainResult.Failure("Author ID cannot be empty.");

        if (_favoriteAuthorIds.Contains(authorId))
            return DomainResult.Failure("Author is already in favorites.");

        if (_favoriteAuthorIds.Count >= 20) // Reasonable limit
            return DomainResult.Failure("Maximum number of favorite authors reached.");

        _favoriteAuthorIds.Add(authorId);
        return DomainResult.Success();
    }

    public DomainResult RemoveFavoriteAuthor(Guid authorId)
    {
        if (!_favoriteAuthorIds.Contains(authorId))
            return DomainResult.Failure("Author not found in favorites.");

        _favoriteAuthorIds.Remove(authorId);
        return DomainResult.Success();
    }

    public DomainResult UpdateNotificationSettings(bool email, bool sms, bool push)
    {
        EmailNotifications = email;
        SmsNotifications = sms;
        PushNotifications = push;
        return DomainResult.Success();
    }

    public DomainResult SetReadingGoal(int booksPerYear)
    {
        if (booksPerYear <= 0)
            return DomainResult.Failure("Reading goal must be greater than zero books per year.");

        if (booksPerYear > 500) // Reasonable upper limit
            return DomainResult.Failure("Reading goal too high. Please set a realistic goal.");

        CurrentReadingGoal = new ReadingGoal(booksPerYear, DateTime.UtcNow.Year);
        return DomainResult.Success();
    }
}