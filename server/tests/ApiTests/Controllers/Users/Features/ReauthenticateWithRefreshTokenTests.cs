using Api.Common.Exceptions;
using Api.Common.ExtensionMethods;
using Api.Controllers.Users.Common.Features;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using Fixtures;
using Moq;

namespace ApiTests.Controllers.Users.Features;

public class ReauthenticateWithRefreshTokenTests : IClassFixture<DatabaseFixture>
{
    Mock<IJwtHelper> _jwtHelperMock = null!;
    DatabaseFixture _databaseFixture;

    public ReauthenticateWithRefreshTokenTests(DatabaseFixture databaseFixture)
    {
        _jwtHelperMock = new Mock<IJwtHelper>();
        _databaseFixture = databaseFixture;
    }

    [Fact]
    public async Task AnExceptionShouldBeThrownIfTheRefreshTokenIsNotFound()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        ReauthenticateWithRefreshTokenHandler handler = new ReauthenticateWithRefreshTokenHandler(
            _jwtHelperMock.Object,
            appDbContext
        );

        await Assert.ThrowsAsync<ApiException>(async () => await handler.Handle("refreshToken"));
    }

    [Fact]
    public async Task AnExceptionShouldBeThrownIfTheRefreshTokenIsExpired()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        User user = new User() { FiatCurrencyType = appDbContext.FiatCurrencyTypes.First() };
        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();

        string nonHashedRefreshToken = "token";
        RefreshToken token = new RefreshToken
        {
            UserId = user.Id,
            Token = nonHashedRefreshToken.ToSHA512(),
            Expires = DateTime.UtcNow.AddMinutes(-10),
        };
        appDbContext.RefreshTokens.Add(token);
        appDbContext.SaveChanges();

        ReauthenticateWithRefreshTokenHandler handler = new ReauthenticateWithRefreshTokenHandler(
            _jwtHelperMock.Object,
            appDbContext
        );

        await Assert.ThrowsAsync<ApiException>(async () => await handler.Handle(nonHashedRefreshToken));
    }

    [Fact]
    public async Task ReauthenticationShouldSucceed()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        User user = new User() { FiatCurrencyType = appDbContext.FiatCurrencyTypes.First() };
        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();

        string nonHashedRefreshToken = "refreshToken";
        RefreshToken refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = nonHashedRefreshToken.ToSHA512(),
            Expires = DateTime.UtcNow.AddHours(1),
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
        Assert.Equal(user.Id, result.UserId);
        // ensure the refresh token gets deleted
        Assert.True(
            appDbContext.RefreshTokens.FirstOrDefault(x => x.Token == nonHashedRefreshToken.ToSHA512()) is null
        );
    }
}
