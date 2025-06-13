using Core.Enums.Book;

namespace Application.Dtos.BookCopyDto;

public class UpdateBookCopyDto
{
    public Guid Id { get; set; }
    public string? Barcode { get; set; }
    public BookCopyStatus Status { get; set; }
    public string? Location { get; set; }
    public BookCondition Condition { get; set; }
    public DateTime? LastInventoryDate { get; set; }
    public string? Notes { get; set; }
}