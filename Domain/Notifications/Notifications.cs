// Domain/Notifications/Notification.cs

using Core.BaseEntities;
using Core.Enums;
using Core.Enums.Notification;

namespace Core.Notifications;

public class Notification : AuditEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationType Type { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public DateTime? ScheduledFor { get; private set; }
    public bool IsSent { get; private set; }
    public DateTime? SentAt { get; private set; }
    public string Channel { get; private set; } 
    
    private Notification() { }
    
    public static DomainResult<Notification> Create(Guid userId, string title, string message, NotificationType type)
    {
        var errors = new List<string>();

        if (userId == Guid.Empty)
            errors.Add("User ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(title))
            errors.Add("Title cannot be empty.");

        if (string.IsNullOrWhiteSpace(message))
            errors.Add("Message cannot be empty.");

        if (errors.Any())
            return DomainResult<Notification>.Failure(errors);

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Priority = NotificationPriority.Normal,
            IsRead = false,
            IsSent = false,
            Channel = "Default"
        };

        return DomainResult<Notification>.Success(notification);
    }

    public DomainResult MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        return DomainResult.Success();
    }

    public DomainResult MarkAsSent()
    {
        if (IsSent)
            return DomainResult.Failure("Notification has already been sent.");

        IsSent = true;
        SentAt = DateTime.UtcNow;
        return DomainResult.Success();
    }

    public DomainResult Schedule(DateTime scheduledFor)
    {
        if (scheduledFor <= DateTime.UtcNow)
            return DomainResult.Failure("Scheduled time must be in the future.");

        ScheduledFor = scheduledFor;
        return DomainResult.Success();
    }

    public DomainResult UpdateMessage(string title, string message)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
            return DomainResult.Failure("Title and message cannot be empty.");

        Title = title;
        Message = message;
        return DomainResult.Success();
    }
}