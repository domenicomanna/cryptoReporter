using Moq;
using Api.Controllers.Users.Features;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using Api.Common.ExtensionMethods;
using Api.Common.Exceptions;

namespace ApiTests.Controllers.Features.Users;

[TestClass]
public class ResetPasswordStepTwoHandlerTests
{
    AppDbContextCreator _appDbContextCreator = null!;
    Mock<IPasswordHasher> _passwordHasherMock = null!;

    [TestInitialize]
    public void SetUp()
    {
        _appDbContextCreator = new AppDbContextCreator();
        _passwordHasherMock = new Mock<IPasswordHasher>();
    }

    [TestMethod]
    public async Task AnExceptionShouldBeThrownIfTheResetTokenIsNotFound()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        User user = new User
        {
            Id = 1,
            Email = "hello@gmail.com",
            FiatCurrencyType = appDbContext.FiatCurrencyTypes.First()
        };
        appDbContext.Users.Add(user);
        await appDbContext.SaveChangesAsync();

        string newPassword = "hello1";

        ResetPasswordStepTwoRequest request = new ResetPasswordStepTwoRequest
        {
            ResetPasswordToken = "test",
            NewPassword = newPassword,
            ConfirmedNewPassword = newPassword
        };

        ResetPasswordStepTwoHandler handler = new ResetPasswordStepTwoHandler(appDbContext, _passwordHasherMock.Object);

        await Assert.ThrowsExceptionAsync<ApiException>(async () => await handler.Handle(request));
    }

    [TestMethod]
    public async Task AnExceptionShouldBeThrownIfTheResetTokenIsExpired()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        User user = new User
        {
            Id = 1,
            Email = "hello@gmail.com",
            FiatCurrencyType = appDbContext.FiatCurrencyTypes.First()
        };
        string token = "test";
        PasswordResetToken passwordResetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token.ToSHA512(),
            Expiration = DateTime.UtcNow.AddHours(-1)
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
            ConfirmedNewPassword = newPassword
        };

        ResetPasswordStepTwoHandler handler = new ResetPasswordStepTwoHandler(appDbContext, _passwordHasherMock.Object);

        await Assert.ThrowsExceptionAsync<ApiException>(async () => await handler.Handle(request));
    }

    [TestMethod]
    public async Task PasswordShouldBeSuccessfullyReset()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        User user = new User
        {
            Id = 1,
            Email = "hello@gmail.com",
            FiatCurrencyType = appDbContext.FiatCurrencyTypes.First()
        };
        string token = "test";
        PasswordResetToken passwordResetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token.ToSHA512(),
            Expiration = DateTime.UtcNow.AddHours(1)
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
            ConfirmedNewPassword = newPassword
        };

        ResetPasswordStepTwoHandler handler = new ResetPasswordStepTwoHandler(appDbContext, _passwordHasherMock.Object);
        await handler.Handle(request);

        Assert.AreEqual(newHashedPassword, appDbContext.Users.First(x => x.Id == user.Id).Password);
    }
}
