using Core.BaseEntities;

namespace Core.Books;

public class Genre : AuditEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    
    public ICollection<Book>? Books { get; set; }
}