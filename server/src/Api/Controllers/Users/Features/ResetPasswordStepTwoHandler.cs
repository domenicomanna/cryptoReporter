using Api.Common.Attributes;
using Api.Common.Exceptions;
using Api.Common.ExtensionMethods;
using Api.Common.ExtensionMethods.ValidationRules;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Users.Features;

public class ResetPasswordStepTwoRequest
{
    public string ResetPasswordToken { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmedNewPassword { get; set; } = string.Empty;
}

public class ResetPasswordStepTwoRequestValidator : AbstractValidator<ResetPasswordStepTwoRequest>
{
    public ResetPasswordStepTwoRequestValidator()
    {
        RuleFor(x => x.ResetPasswordToken).NotEmpty().NotNull();
        RuleFor(x => x.NewPassword).Password();
        RuleFor(x => x.ConfirmedNewPassword).Equal(x => x.NewPassword);
    }
}

[Inject]
public class ResetPasswordStepTwoHandler
{
    private readonly AppDbContext _appDbcontext;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordStepTwoHandler(AppDbContext bodyFitTrackerContext, IPasswordHasher passwordHasher)
    {
        _appDbcontext = bodyFitTrackerContext;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ResetPasswordStepTwoRequest resetPasswordStepTwoRequest)
    {
        PasswordResetToken? passwordResetToken = await _appDbcontext
            .PasswordResetTokens.Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == resetPasswordStepTwoRequest.ResetPasswordToken.ToSHA512());
        if (passwordResetToken is null)
            throw new ApiException { HttpStatusCode = System.Net.HttpStatusCode.Unauthorized };

        if (passwordResetToken.Expiration < DateTime.UtcNow)
        {
            throw new ApiException { HttpStatusCode = System.Net.HttpStatusCode.Unauthorized };
        }

        User user = passwordResetToken.User;
        string hashedPassword = _passwordHasher.HashPassword(resetPasswordStepTwoRequest.NewPassword);

        user.Password = hashedPassword;

        _appDbcontext.PasswordResetTokens.Remove(passwordResetToken);
        _appDbcontext.SaveChanges();

        return;
    }
}
