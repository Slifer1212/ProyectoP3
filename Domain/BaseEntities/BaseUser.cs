namespace Core.BaseEntities;

public abstract class BaseUser : Person
{
    public Guid Id { get; private set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public virtual string? Role { get; } 
    
    public abstract bool CanAccessResource(string resource);

}