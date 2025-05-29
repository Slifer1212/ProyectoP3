using Core.BaseEntities;

namespace Core.Users;

public class Librarian : BaseUser
{
    
    public override string Role { get; } = "Librarian";

    public override bool CanAccessResource(string resource) 
    {
        return resource.StartsWith("LibrarySection");
    }
}