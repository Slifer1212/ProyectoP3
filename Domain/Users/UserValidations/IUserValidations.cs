namespace Core.Users.UserValidations;

public interface IUserValidations
{
    bool IsValidUserName(string? userName);
    bool IsValidPassword(string? password);
    bool IsValidEmail(string? email);
    bool IsValidPhoneNumber(string? phoneNumber);
}