using Core.Books;
using Core.Users;

namespace Core.Loans;

public class Reservation
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual Book Book { get; set; }
    public virtual Member Member { get; set; }
}