using System.Security.Cryptography;
using System.Text;

namespace Api.Common.ExtensionMethods;

public static class StringExtensionMethods
{
    public static string ToSHA512(this string value)
    {
        using SHA512 sha512 = SHA512.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        byte[] hash = sha512.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
