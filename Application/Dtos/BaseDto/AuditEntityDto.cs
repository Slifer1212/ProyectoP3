namespace Application.Dtos.BaseDto;

public class AuditEntityDto
{
    public DateTime CreatedAt { get; set; } 
    public bool? IsDeleted { get; protected set; } 
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public AuditEntityDto()
    {
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
}