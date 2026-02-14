using Api.Common.Exceptions;
using Api.Common.ExtensionMethods;
using Api.Controllers.Users.Features;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using Fixtures;
using Moq;

namespace ApiTests.Controllers.Features.Users;

public class ResetPasswordStepTwoHandlerTests : IClassFixture<DatabaseFixture>
{
    DatabaseFixture _databaseFixture;
    Mock<IPasswordHasher> _passwordHasherMock = null!;

    public ResetPasswordStepTwoHandlerTests(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        _passwordHasherMock = new Mock<IPasswordHasher>();
    }

    [Fact]
    public async Task AnExceptionShouldBeThrownIfTheResetTokenIsNotFound()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        User user = new User
        {
            Id = 1,
            Email = "hello@gmail.com",
            FiatCurrencyType = appDbContext.FiatCurrencyTypes.First(),
        };
        appDbContext.Users.Add(user);
        await appDbContext.SaveChangesAsync();

        string newPassword = "hello1";

        ResetPasswordStepTwoRequest request = new ResetPasswordStepTwoRequest
        {
            ResetPasswordToken = "test",
            NewPassword = newPassword,
            ConfirmedNewPassword = newPassword,
        };

        ResetPasswordStepTwoHandler handler = new ResetPasswordStepTwoHandler(appDbContext, _passwordHasherMock.Object);

        await Assert.ThrowsAsync<ApiException>(async () => await handler.Handle(request));
    }

    [Fact]
    public async Task AnExceptionShouldBeThrownIfTheResetTokenIsExpired()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        User user = new User
        {
            Id = 1,
            Email = "hello@gmail.com",
            FiatCurrencyType = appDbContext.FiatCurrencyTypes.First(),
        };
        string token = "test";
        PasswordResetToken passwordResetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token.ToSHA512(),
            Expiration = DateTime.UtcNow.AddHours(-1),
        };

        appDbContext.Users.Add(user);
        appDbContext.PasswordResetTokens.Add(passwordResetToken);
        await appDbContext.SaveChangesAsync();

        string newPassword = "hello1";
        string newHashedPassword = "xyzxyz";

        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns(newHashedPassword);

        ResetPasswordStepTwoRequest request = new ResetPasswordStepTwoRequest
        {
            ResetPasswordToken = token,
            NewPassword = newPassword,
            ConfirmedNewPassword = newPassword,
        };

        ResetPasswordStepTwoHandler handler = new ResetPasswordStepTwoHandler(appDbContext, _passwordHasherMock.Object);

        await Assert.ThrowsAsync<ApiException>(async () => await handler.Handle(request));
    }

    [Fact]
    public async Task PasswordShouldBeSuccessfullyReset()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        User user = new User
        {
            Id = 1,
            Email = "hello@gmail.com",
            FiatCurrencyType = appDbContext.FiatCurrencyTypes.First(),
        };
        string token = "test";
        PasswordResetToken passwordResetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token.ToSHA512(),
            Expiration = DateTime.UtcNow.AddHours(1),
        };

        appDbContext.Users.Add(user);
        appDbContext.PasswordResetTokens.Add(passwordResetToken);
        await appDbContext.SaveChangesAsync();

        string newPassword = "hello1";
        string newHashedPassword = "xyzxyz";

        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns(newHashedPassword);

        ResetPasswordStepTwoRequest request = new ResetPasswordStepTwoRequest
        {
            ResetPasswordToken = token,
            NewPassword = newPassword,
            ConfirmedNewPassword = newPassword,
        };

        ResetPasswordStepTwoHandler handler = new ResetPasswordStepTwoHandler(appDbContext, _passwordHasherMock.Object);
        await handler.Handle(request);

        Assert.Equal(newHashedPassword, appDbContext.Users.First(x => x.Id == user.Id).Password);
    }
}
