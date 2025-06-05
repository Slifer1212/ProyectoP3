using Core.BaseEntities;
using Core.Enums;
using Core.Enums.Book;

namespace Core.Books;

public class BookCopy : AuditEntity
{
    public Guid Id { get; private set; }
    public Guid BookId { get; private set; }
    public string Barcode { get; private set; }
    public BookCopyStatus Status { get; private set; }
    public string Location { get; private set; }  
    public BookCondition Condition { get; private set; }  
    public DateTime? LastInventoryDate { get; private set; }
    public string Notes { get; private set; }
    
    private BookCopy() { }

    public static DomainResult<BookCopy> Create(
        Guid bookId, string barcode, string location,
        BookCopyStatus status,
        BookCondition condition,
        string notes = null, DateTime? lastInventoryDate = null
        
    )
    {
        var errors = new List<string>();
        
        if (bookId == Guid.Empty)
            errors.Add("Book ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(barcode))
            errors.Add("Barcode cannot be empty.");

        if (string.IsNullOrWhiteSpace(location))
            errors.Add("Location cannot be empty.");

        if (status == BookCopyStatus.Withdrawn)
            errors.Add("Status cannot be withdrawn on creation.");

        if (errors.Any())
            return DomainResult<BookCopy>.Failure(errors);

        var bookCopy = new BookCopy
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            Barcode = barcode,
            Location = location,
            Status = status,
            Condition = condition,
            Notes = notes,
            LastInventoryDate = lastInventoryDate ?? DateTime.UtcNow
        };
        
        return DomainResult<BookCopy>.Success(bookCopy);

    }

    public bool CanBeLent()
    {
        return Status == BookCopyStatus.Available || Status == BookCopyStatus.Reserved;
    }

    public bool IsAvailable()
    {
        return Status == BookCopyStatus.Available;
    }

    public DomainResult Loaned()
    {
        if (Status == BookCopyStatus.OnLoan)
            return DomainResult.Failure("This copy is already loan out.");

        if (Status != BookCopyStatus.Available)
            return DomainResult.Failure("This copy cannot be lent at this time.");

        Status = BookCopyStatus.OnLoan;
        return DomainResult.Success();
    }

    public DomainResult Return()
    {
        if (Status == BookCopyStatus.Available)
            return DomainResult.Failure("This copy is already available.");

        if (Status != BookCopyStatus.OnLoan)
            return DomainResult.Failure("This copy cannot be returned at this time.");

        Status = BookCopyStatus.Available;
        return DomainResult.Success();
    }

    public DomainResult Reserve()
    {
        if (Status == BookCopyStatus.Reserved)
            return DomainResult.Failure("This copy is already reserved.");

        if (Status != BookCopyStatus.Available)
            return DomainResult.Failure("This copy cannot be reserved at this time.");

        Status = BookCopyStatus.Reserved;
        return DomainResult.Success();
    }

    public DomainResult MarkAsLost()
    {
        if (Status == BookCopyStatus.Lost)
            return DomainResult.Failure("This copy is already marked as lost.");

        Status = BookCopyStatus.Lost;
        return DomainResult.Success();
    }

    public DomainResult MarkAsDamaged(BookCondition condition, string notes)
    {
        if (Status == BookCopyStatus.Damaged)
            return DomainResult.Failure("This copy is already marked as damaged.");

        Status = BookCopyStatus.Damaged;
        Condition = condition;
        Notes = notes;
        return DomainResult.Success();
    }

    public DomainResult SendToMaintenance(BookCopyStatus copyStatus, string notes)
    {
        if (Status == BookCopyStatus.InMaintenance)
            return DomainResult.Failure("This copy is already in maintenance.");

        Status = BookCopyStatus.InMaintenance;
        Notes = notes;
        return DomainResult.Success();
    }

    public DomainResult UpdateInventoryDate()
    {
        if (Status == BookCopyStatus.Withdrawn)
            return DomainResult.Failure("This copy is withdrawn and cannot be updated.");

        LastInventoryDate = DateTime.UtcNow;
        return DomainResult.Success();
    }
}