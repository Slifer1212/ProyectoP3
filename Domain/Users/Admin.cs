
using Core.BaseEntities;

namespace Core.Users;

public class Admin : LibraryUser
{
    public override string Role { get; } = "Admin";
    
    private Admin() { }

    public static DomainResult<Admin> Create(Guid identityUserId, string firstName, string lastName, string email)
    {
        var errors = new List<string>();
        
        var userValidations = new UserValidations.UserValidations();
        
        if (!userValidations.IsValidEmail(email))
            errors.Add("Invalid email format.");
        
        if (!userValidations.IsValidUserName(firstName))
            errors.Add("Invalid first name format.");
        
        if (!userValidations.IsValidUserName(lastName))
            errors.Add("Invalid last name format.");
        
        if (errors.Any())
            return DomainResult<Admin>.Failure(errors);

        var admin = new Admin
        {
            Id = Guid.NewGuid(),
            IdentityUserId = identityUserId,
            Name = firstName,
            LastName = lastName,
            Email = email,
        };
        return DomainResult<Admin>.Success(admin);
    }
    
    public override bool CanAccessResource(string resource) => true;
}