using System.Text;
using FluentValidation;
using Api.Common.Attributes;
using Api.Utils.Emailing;
using Api.Database;
using Api.Domain.Models;
using System.Security.Cryptography;
using Api.Common.ExtensionMethods;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Users.Features;

public class ResetPasswordStepOneRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordStepOneRequestValidator : AbstractValidator<ResetPasswordStepOneRequest>
{
    public ResetPasswordStepOneRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().NotNull();
    }
}

[Inject]
public class ResetPasswordStepOneHandler
{
    private readonly AppDbContext _appDbContext;
    private readonly IEmailSender _emailSender;

    public ResetPasswordStepOneHandler(AppDbContext bodyFitTrackerContext, IEmailSender emailSender)
    {
        this._appDbContext = bodyFitTrackerContext;
        this._emailSender = emailSender;
    }

    public async Task Handle(ResetPasswordStepOneRequest resetPasswordStepOneRequest)
    {
        User? user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Email == resetPasswordStepOneRequest.Email);
        if (user == null)
            return;

        string resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)).Replace("/", "-");

        DateTime expiration = DateTime.UtcNow.AddHours(1);
        PasswordResetToken passwordReset = new PasswordResetToken
        {
            UserId = user.Id,
            Expiration = expiration,
            Token = resetToken.ToSHA512()
        };

        _appDbContext.PasswordResetTokens.Add(passwordReset);
        await _appDbContext.SaveChangesAsync();

        EmailMessage emailMessage = CreateEmailMessage(user, resetToken);
        _emailSender.SendEmail(emailMessage);
    }

    private EmailMessage CreateEmailMessage(User user, string resetToken)
    {
        EmailMessage emailMessage = new EmailMessage
        {
            Subject = "Reset Your Password",
            To = new List<string> { user.Email }
        };
        StringBuilder htmlBody = new StringBuilder();
        string clientAppUrl = DotNetEnv.Env.GetString("ClientAppUrl");
        htmlBody.Append("<p>Hi, a request was received to reset your password.<p>");
        htmlBody.Append(
            $"<p>Please follow <a href='{clientAppUrl}/reset-password-step-two/{resetToken}' target='_blank'>this link</a> to reset your password."
        );
        emailMessage.HtmlBody = htmlBody.ToString();

        return emailMessage;
    }
}
