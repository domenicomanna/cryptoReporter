using System.Security.Cryptography;
using System.Text;
using FluentValidation;

namespace Api.Common.ExtensionMethods.ValidationRules;

public static class PasswordExtension
{
    public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> builder)
    {
        return builder.NotEmpty().NotNull().MinimumLength(6);
    }
}
