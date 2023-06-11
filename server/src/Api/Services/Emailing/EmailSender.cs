using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Api.Services.Emailing;

public interface IEmailSender
{
    void SendEmail(EmailMessage emailMessage);
}

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;

    public EmailSender()
    {
        _emailSettings = new EmailSettings
        {
            From = DotNetEnv.Env.GetString("EmailAddress"),
            UserName = DotNetEnv.Env.GetString("EmailAddress"),
            Password = DotNetEnv.Env.GetString("EmailPassword"),
            SmtpServer = DotNetEnv.Env.GetString("EmailSmtpServer"),
            Port = DotNetEnv.Env.GetInt("EmailPort")
        };
    }

    public void SendEmail(EmailMessage emailMessage)
    {
        MimeMessage message = CreateMimeMessage(emailMessage);
        using SmtpClient smtpClient = new SmtpClient();
        smtpClient.Connect(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
        smtpClient.Authenticate(_emailSettings.UserName, _emailSettings.Password);
        smtpClient.Send(message);
    }

    private MimeMessage CreateMimeMessage(EmailMessage emailMessage)
    {
        MimeMessage mimeMessage = new MimeMessage();
        mimeMessage.From.Add(MailboxAddress.Parse(_emailSettings.From));
        mimeMessage.To.AddRange(emailMessage.To.Select(x => MailboxAddress.Parse(x)));
        mimeMessage.Subject = emailMessage.Subject;

        BodyBuilder builder = new BodyBuilder();
        builder.TextBody = emailMessage.TextBody;
        builder.HtmlBody = emailMessage.HtmlBody;

        mimeMessage.Body = builder.ToMessageBody();

        return mimeMessage;
    }
}
