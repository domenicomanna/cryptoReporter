using Moq;
using Api.Controllers.Users.Features;
using Api.Utils.Emailing;
using Api.Database;
using Api.Domain.Models;
using Fixtures;

namespace ApiTests.Controllers.Features.Users;

public class ResetPasswordStepOneHandlerTests : IClassFixture<DatabaseFixture>
{
    DatabaseFixture _databaseFixture;
    IEmailSender _emailSender = null!;

    public ResetPasswordStepOneHandlerTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;

        var emailSenderMock = new Mock<IEmailSender>();
        emailSenderMock.Setup(x => x.SendEmail(It.IsAny<EmailMessage>()));

        _emailSender = emailSenderMock.Object;
    }

    [Fact]
    public async Task NothingShouldHappenIfTheEmailIsNotFound()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        ResetPasswordStepOneRequest request = new ResetPasswordStepOneRequest { Email = "test@gmail.com" };

        ResetPasswordStepOneHandler handler = new ResetPasswordStepOneHandler(appDbContext, _emailSender);
        await handler.Handle(request);

        Assert.Equal(0, appDbContext.PasswordResetTokens.Count());
    }

    [Fact]
    public async Task IfTheEmailIsFoundAPasswordResetTokenShouldBeCreated()
    {
        string email = "test@gmail.com";

        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        appDbContext.Users.Add(new User { Email = email, FiatCurrencyType = appDbContext.FiatCurrencyTypes.First() });
        await appDbContext.SaveChangesAsync();

        ResetPasswordStepOneRequest request = new ResetPasswordStepOneRequest { Email = email };

        ResetPasswordStepOneHandler handler = new ResetPasswordStepOneHandler(appDbContext, _emailSender);
        await handler.Handle(request);

        Assert.Equal(1, appDbContext.PasswordResetTokens.Count());
    }
}
