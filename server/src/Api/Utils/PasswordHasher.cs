using BC = BCrypt.Net.BCrypt;

namespace Api.Utils;

public interface IPasswordHasher
{
    public string HashPassword(string plainTextPassword);
    public bool VerifyPassword(string plainTextPassword, string hashedPassowrd);
}

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string plainTextPassword)
    {
        return BC.HashPassword(plainTextPassword);
    }

    public bool VerifyPassword(string plainTextPassword, string hashedPassowrd)
    {
        return BC.Verify(plainTextPassword, hashedPassowrd);
    }
}
