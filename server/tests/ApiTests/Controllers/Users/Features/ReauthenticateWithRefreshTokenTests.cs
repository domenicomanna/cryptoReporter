using Api.Common.Exceptions;
using Api.Controllers.Users.Common.Features;
using Api.Database;
using Api.Domain.Models;
using Api.Common.ExtensionMethods;
using Api.Services;
using Moq;

namespace ApiTests.Controllers.Users.Features;

[TestClass]
public class ReauthenticateWithRefreshTokenTests
{
    AppDbContextCreator _appDbContextCreator = null!;
    Mock<IJwtHelper> _jwtHelperMock = null!;

    [TestInitialize]
    public void SetUp()
    {
        _appDbContextCreator = new AppDbContextCreator();
        _jwtHelperMock = new Mock<IJwtHelper>();
    }

    [TestMethod]
    public async Task AnExceptionShouldBeThrownIfTheRefreshTokenIsNotFound()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        ReauthenticateWithRefreshTokenHandler handler = new ReauthenticateWithRefreshTokenHandler(
            _jwtHelperMock.Object,
            appDbContext
        );

        await Assert.ThrowsExceptionAsync<ApiException>(async () => await handler.Handle("refreshToken"));
    }

    [TestMethod]
    public async Task AnExceptionShouldBeThrownIfTheRefreshTokenIsExpired()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        User user = new User();
        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();

        string nonHashedRefreshToken = "token";
        RefreshToken token = new RefreshToken
        {
            UserId = user.Id,
            Token = nonHashedRefreshToken.ToSHA512(),
            Expires = DateTime.UtcNow.AddMinutes(-10)
        };
        appDbContext.RefreshTokens.Add(token);
        appDbContext.SaveChanges();

        ReauthenticateWithRefreshTokenHandler handler = new ReauthenticateWithRefreshTokenHandler(
            _jwtHelperMock.Object,
            appDbContext
        );

        await Assert.ThrowsExceptionAsync<ApiException>(async () => await handler.Handle(nonHashedRefreshToken));
    }

    [TestMethod]
    public async Task ReauhenticationShouldSucceed()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        User user = new User();
        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();

        string nonHashedRefreshToken = "refreshToken";
        RefreshToken refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = nonHashedRefreshToken.ToSHA512(),
            Expires = DateTime.UtcNow.AddHours(1)
        };
        appDbContext.RefreshTokens.Add(refreshToken);
        appDbContext.SaveChanges();

        _jwtHelperMock.Setup(x => x.CreateAccessToken(It.IsAny<User>())).Returns("token");
        _jwtHelperMock
            .Setup(x => x.CreateRefreshToken(It.IsAny<User>()))
            .Returns((User user) => (new RefreshToken { UserId = user.Id }, "newRefreshToken"));

        ReauthenticateWithRefreshTokenHandler handler = new ReauthenticateWithRefreshTokenHandler(
            _jwtHelperMock.Object,
            appDbContext
        );

        (ReauthenticateWithRefreshTokenResult result, string newNonHashedRefreshToken) = await handler.Handle(
            nonHashedRefreshToken
        );
        Assert.AreEqual(user.Id, result.UserId);
        // ensure the refresh token gets deleted
        Assert.IsTrue(
            appDbContext.RefreshTokens.FirstOrDefault(x => x.Token == nonHashedRefreshToken.ToSHA512()) is null
        );
    }
}
