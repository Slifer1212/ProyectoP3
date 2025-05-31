namespace Core.BaseEntities;

public abstract class AuditEntity
{
    public DateTime CreatedAt { get; set; }  = DateTime.UtcNow;
    public bool? IsDeleted { get; protected set; } = false;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    
}