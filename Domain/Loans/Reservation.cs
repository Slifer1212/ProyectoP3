using Core.BaseEntities;
using Core.Enums;
using Core.Enums.Loans;

namespace Core.Loans;

public class Reservation : AuditEntity
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public Guid MemberId { get; private set; }
    public DateTime ReservationDate { get; private set; }
    public DateTime ExpirationDate { get; private set; }
    public ReservationStatus Status { get; private set; }
    public int QueuePosition { get; private set; }
    public DateTime? NotifiedDate { get; private set; }
    public DateTime? ReadyDate { get; private set; }
    public string Notes { get; private set; }

    private Reservation()
    {
    }

    public static DomainResult<Reservation> Create(Guid bookId, Guid memberId, int reservationDays = 7)
    {
        var errors = new List<string>();
        if (bookId == Guid.Empty)
            errors.Add("Book ID cannot be empty.");

        if (memberId == Guid.Empty)
            errors.Add("Member ID cannot be empty.");

        if (reservationDays <= 0)
            errors.Add("Reservation days must be greater than zero.");

        if (errors.Any())
            return DomainResult<Reservation>.Failure(errors);

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            MemberId = memberId,
            ReservationDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(reservationDays),
            Status = ReservationStatus.Pending,
            QueuePosition = 1 
        };

        return DomainResult<Reservation>.Success(reservation);
    }

    public bool IsExpired()
    {
        return ExpirationDate < DateTime.UtcNow &&
               (Status != ReservationStatus.Canceled &&
                Status != ReservationStatus.Fulfilled &&
                Status != ReservationStatus.Expired);
    }

    public bool IsReady()
    {
        return Status == ReservationStatus.Ready && ReadyDate.HasValue;
    }

    public DomainResult Activate()
    {
        if (Status != ReservationStatus.Pending)
            return DomainResult.Failure("Reservation can only be activated when in pending status.");

        Status = ReservationStatus.Active;
        return DomainResult.Success();
    }

    public DomainResult MarkAsReady(Guid availableCopyId)
    {
        if (Status != ReservationStatus.Active && Status != ReservationStatus.Pending)
            return DomainResult.Failure("Only active or pending reservations can be marked as ready.");

        if (availableCopyId == Guid.Empty)
            return DomainResult.Failure("Available copy ID cannot be empty.");

        Status = ReservationStatus.Ready;
        ReadyDate = DateTime.UtcNow;
        Notes = string.IsNullOrEmpty(Notes)
            ? $"Copy {availableCopyId} available for pickup"
            : $"{Notes}\nCopy {availableCopyId} available for pickup";

        return DomainResult.Success();
    }

    public DomainResult Fulfill(Guid loanId)
    {
        if (Status != ReservationStatus.Ready)
            return DomainResult.Failure("Only ready reservations can be fulfilled.");

        if (loanId == Guid.Empty)
            return DomainResult.Failure("Loan ID cannot be empty.");

        Status = ReservationStatus.Fulfilled;
        Notes = string.IsNullOrEmpty(Notes)
            ? $"Fulfilled with loan {loanId}"
            : $"{Notes}\nFulfilled with loan {loanId}";

        return DomainResult.Success();
    }

    public DomainResult Cancel()
    {
        if (Status == ReservationStatus.Fulfilled)
            return DomainResult.Failure("Cannot cancel a fulfilled reservation.");

        if (Status == ReservationStatus.Canceled)
            return DomainResult.Failure("Reservation is already canceled.");

        Status = ReservationStatus.Canceled;
        return DomainResult.Success();
    }

    public DomainResult Expire()
    {
        if (Status == ReservationStatus.Fulfilled || Status == ReservationStatus.Canceled)
            return DomainResult.Failure("Cannot expire a fulfilled or canceled reservation.");

        if (Status == ReservationStatus.Expired)
            return DomainResult.Failure("Reservation is already expired.");

        if (!IsExpired())
            return DomainResult.Failure("Reservation has not yet expired.");

        Status = ReservationStatus.Expired;
        return DomainResult.Success();
    }

    public DomainResult UpdateQueuePosition(int newPosition)
    {
        if (newPosition <= 0)
            return DomainResult.Failure("Queue position must be greater than zero.");

        if (Status != ReservationStatus.Active && Status != ReservationStatus.Pending)
            return DomainResult.Failure("Queue position can only be updated for active or pending reservations.");

        QueuePosition = newPosition;
        return DomainResult.Success();
    }

    public DomainResult ExtendExpiration(int additionalDays)
    {
        if (additionalDays <= 0)
            return DomainResult.Failure("Additional days must be greater than zero.");

        if (Status == ReservationStatus.Fulfilled ||
            Status == ReservationStatus.Canceled ||
            Status == ReservationStatus.Expired)
            return DomainResult.Failure("Cannot extend expired, canceled, or fulfilled reservations.");

        ExpirationDate = ExpirationDate.AddDays(additionalDays);
        return DomainResult.Success();

    }
}