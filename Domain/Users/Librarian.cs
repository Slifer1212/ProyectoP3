
using Core.BaseEntities;

namespace Core.Users;

public class Librarian : LibraryUser
{
    public string Department { get; private set; }
    public DateTime HireDate { get; private set; }
    public override bool CanManageBooks { get;  set; } = true;
    public override bool CanManageUsers { get;  set; } = false;
    
    public override string Role { get; } = "Librarian";

    private Librarian() { }

    public static DomainResult<Librarian> Create(Guid identityUserId, string firstName, string lastName, string email,
        string department)
    {
        var errors = new List<string>();
        
        var userValidations = new UserValidations.UserValidations();
        if (!userValidations.IsValidEmail(email))
            errors.Add("Invalid email format.");
        
        if (!userValidations.IsValidUserName(firstName))
            errors.Add("Invalid first name format.");
        
        if (!userValidations.IsValidUserName(lastName))
            errors.Add("Invalid last name format.");
        
        if (string.IsNullOrWhiteSpace(department))
            errors.Add("Department is required.");
        
        if (errors.Any())
            return DomainResult<Librarian>.Failure(errors);
        
        var librarian = new Librarian
        {
            Id = Guid.NewGuid(),
            IdentityUserId = identityUserId,
            Name = firstName,
            LastName = lastName,
            Email = email,
            Department = department,
            HireDate = DateTime.UtcNow
        };
        
        return DomainResult<Librarian>.Success(librarian);
    }
    
    public override bool CanAccessResource(string resource)
    {
        return resource.StartsWith("Library") || resource.StartsWith("Book") || resource.StartsWith("Loan");
    }
}