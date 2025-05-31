namespace Core.Users.UserValidations;
using System.Text.RegularExpressions;

public class UserValidations : IUserValidations
{
    public bool IsValidUserName(string? userName)
    {
        // Letras, números, entre 3 y 20 caracteres
        return !string.IsNullOrWhiteSpace(userName) &&
               Regex.IsMatch(userName, @"^[a-zA-Z0-9]{3,20}$");
    }

    public bool IsValidPassword(string? password)
    {
        // Al menos 6 caracteres, una mayúscula, una minúscula y un número
        return !string.IsNullOrWhiteSpace(password) &&
               Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$");
    }

    public bool IsValidEmail(string? email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
               Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public bool IsValidPhoneNumber(string? phoneNumber)
    {
        // Permite números con +, espacios, guiones, paréntesis
        return !string.IsNullOrWhiteSpace(phoneNumber) &&
               Regex.IsMatch(phoneNumber, @"^\+?\d{1,3}?[-. (]?\d{1,4}?[-. )]?\d{3,4}[-. ]?\d{3,4}$");
    }
}