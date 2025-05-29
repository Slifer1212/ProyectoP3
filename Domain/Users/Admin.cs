using Core.BaseEntities;

namespace Core.Users;

public class Admin : BaseUser
{
    
    public override string Role { get; } = "Admin";
    public override bool CanAccessResource(string resource) => true; 
    
}