namespace Core.BaseEntities;

public abstract class LibraryUser : Person
{
    public Guid Id { get; protected set; }
    public Guid IdentityUserId { get; protected set; } 
    public string Email { get; protected set; }
    public string PhoneNumber { get; protected set; }
    
    public virtual bool CanManageBooks { get;  set; }
    public virtual bool CanManageUsers { get;  set; } 

    public abstract string Role { get; }
    
    public abstract bool CanAccessResource(string resource);
    
    protected LibraryUser() { }
}