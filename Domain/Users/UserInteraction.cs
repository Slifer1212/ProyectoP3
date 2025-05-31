// Domain/Users/UserInteraction.cs

using Core.BaseEntities;
using Core.Enums;

namespace Core.Users;

public class UserInteraction : AuditEntity
{
    public Guid Id { get; private set; }
    public Guid MemberId { get; private set; }
    public Guid BookId { get; private set; }
    public InteractionType Type { get; private set; }
    public DateTime InteractionDate { get; private set; }
    public int? Rating { get; private set; } // 1-5 stars
    public string Review { get; private set; }
    public int TimeSpentMinutes { get; private set; } // Para views
    public string SearchQuery { get; private set; } // Para searches

    private UserInteraction()
    {
    }

    public static DomainResult<UserInteraction> CreateView(Guid memberId, Guid bookId, int timeSpent)
    {
        var errors = new List<string>();
        if (memberId == Guid.Empty)
           errors.Add("Member ID cannot be empty.");

        if (bookId == Guid.Empty)
            errors.Add("Book ID cannot be empty.");

        if (timeSpent <= 0)
            errors.Add("Time spent must be greater than zero.");

        
        if (errors.Any())
            return DomainResult<UserInteraction>.Failure(errors);
        
        var interaction = new UserInteraction
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            BookId = bookId,
            Type = InteractionType.View,
            InteractionDate = DateTime.UtcNow,
            TimeSpentMinutes = timeSpent
        };

        return DomainResult<UserInteraction>.Success(interaction);
    }

    public static DomainResult<UserInteraction> CreateSearch(Guid memberId, Guid bookId, string query)
    {
        var errors = new List<string>();

        if (memberId == Guid.Empty)
            errors.Add("Member ID cannot be empty.");

        if (bookId == Guid.Empty)
            errors.Add("Book ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(query))
            errors.Add("Search query cannot be empty.");

        if (errors.Any())
            return DomainResult<UserInteraction>.Failure(errors);
        
        var interaction = new UserInteraction
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            BookId = bookId,
            Type = InteractionType.Search,
            InteractionDate = DateTime.UtcNow,
            SearchQuery = query
        };

        return DomainResult<UserInteraction>.Success(interaction);
    }

    public static DomainResult<UserInteraction> CreateRating(Guid memberId, Guid bookId, int rating,
        string review = null)
    {
        var errors = new List<string>();

        if (memberId == Guid.Empty)
            errors.Add("Member ID cannot be empty.");

        if (bookId == Guid.Empty)
            errors.Add("Book ID cannot be empty.");

        if (rating < 1 || rating > 5)
            errors.Add("Rating must be between 1 and 5.");

        if (errors.Any())
            return DomainResult<UserInteraction>.Failure(errors);
        
        var interaction = new UserInteraction
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            BookId = bookId,
            Type = InteractionType.Rating,
            InteractionDate = DateTime.UtcNow,
            Rating = rating,
            Review = review
        };

        return DomainResult<UserInteraction>.Success(interaction);
    }

    public static DomainResult<UserInteraction> CreateFavorite(Guid memberId, Guid bookId)
    {
        var errors = new List<string>();
        
        if (memberId == Guid.Empty)
            errors.Add("Member ID cannot be empty.");

        if (bookId == Guid.Empty)
            errors.Add("Book ID cannot be empty.");

        if (errors.Any())
            return DomainResult<UserInteraction>.Failure(errors);
        
        var interaction = new UserInteraction
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            BookId = bookId,
            Type = InteractionType.Favorite,
            InteractionDate = DateTime.UtcNow
        };

        return DomainResult<UserInteraction>.Success(interaction);
    }

    public DomainResult UpdateRating(int rating, string review = null)
    {
        if (Type != InteractionType.Rating)
            return DomainResult.Failure("Can only update rating for rating interaction types.");

        if (rating < 1 || rating > 5)
            return DomainResult.Failure("Rating must be between 1 and 5.");

        Rating = rating;

        if (review != null)
            Review = review;

        return DomainResult.Success();
    }

    public DomainResult UpdateTimeSpent(int additionalMinutes)
    {
        if (Type != InteractionType.View)
            return DomainResult.Failure("Can only update time spent for view interaction types.");

        if (additionalMinutes <= 0)
            return DomainResult.Failure("Additional time must be greater than zero.");

        TimeSpentMinutes += additionalMinutes;

        return DomainResult.Success();
    }
}