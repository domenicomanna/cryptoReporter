using Moq;
using Api.Controllers.Users.Features;
using Api.Utils.Emailing;
using Api.Database;
using Api.Domain.Models;

namespace ApiTests.Controllers.Features.Users;

public class ResetPasswordStepOneHandlerTests
{
    AppDbContextCreator _appDbContextCreator = null!;
    IEmailSender _emailSender = null!;

    public ResetPasswordStepOneHandlerTests()
    {
        _appDbContextCreator = new AppDbContextCreator();

        var emailSenderMock = new Mock<IEmailSender>();
        emailSenderMock.Setup(x => x.SendEmail(It.IsAny<EmailMessage>()));

        _emailSender = emailSenderMock.Object;
    }

    [Fact]
    public async Task NothingShouldHappenIfTheEmailIsNotFound()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        ResetPasswordStepOneRequest request = new ResetPasswordStepOneRequest { Email = "test@gmail.com" };

        ResetPasswordStepOneHandler handler = new ResetPasswordStepOneHandler(appDbContext, _emailSender);
        await handler.Handle(request);

        Assert.Equal(0, appDbContext.PasswordResetTokens.Count());
    }

    [Fact]
    public async Task IfTheEmailIsFoundAPasswordResetTokenShouldBeCreated()
    {
        string email = "test@gmail.com";

        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        appDbContext.Users.Add(new User { Email = email, FiatCurrencyType = appDbContext.FiatCurrencyTypes.First() });
        await appDbContext.SaveChangesAsync();

        ResetPasswordStepOneRequest request = new ResetPasswordStepOneRequest { Email = email };

        ResetPasswordStepOneHandler handler = new ResetPasswordStepOneHandler(appDbContext, _emailSender);
        await handler.Handle(request);

        Assert.Equal(1, appDbContext.PasswordResetTokens.Count());
    }
}
