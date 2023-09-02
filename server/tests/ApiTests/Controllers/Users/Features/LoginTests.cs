using Api.Common.Exceptions;
using Api.Controllers.Users;
using Api.Controllers.Users.Common.Features;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using AutoMapper;
using Moq;

namespace ApiTests.Controllers.Users.Features;

[TestClass]
public class LoginTests
{
    IMapper _mapper = null!;
    AppDbContextCreator _appDbContextCreator = null!;
    Mock<IPasswordHasher> _passwordHasherMock = null!;
    Mock<IJwtHelper> _jwtHelperMock = null!;

    [TestInitialize]
    public void SetUp()
    {
        MapperConfiguration mapperConfiguration = new MapperConfiguration(opts =>
        {
            opts.AddProfile(new UsersMappingProfile());
        });
        _mapper = mapperConfiguration.CreateMapper();

        _appDbContextCreator = new AppDbContextCreator();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtHelperMock = new Mock<IJwtHelper>();
    }

    [TestMethod]
    public async Task AnExceptionShouldBeThrownIfTheUserIsNotFound()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        LoginHandler handler = new LoginHandler(
            _passwordHasherMock.Object,
            _mapper,
            _jwtHelperMock.Object,
            appDbContext
        );
        LoginRequest request = new LoginRequest { Email = "test@gmail.com", Password = "12345" };

        await Assert.ThrowsExceptionAsync<ApiException>(async () => await handler.Handle(request));
    }

    [TestMethod]
    public async Task AnExceptionShouldBeThrownIfTheUsersPasswordIsInvalid()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        User user = new User { Email = "test@gmail.com", FiatCurrencyType = appDbContext.FiatCurrencyTypes.First() };
        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();

        _passwordHasherMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        LoginHandler handler = new LoginHandler(
            _passwordHasherMock.Object,
            _mapper,
            _jwtHelperMock.Object,
            appDbContext
        );
        LoginRequest request = new LoginRequest { Email = user.Email, Password = "12345" };

        await Assert.ThrowsExceptionAsync<ApiException>(async () => await handler.Handle(request));
    }

    [TestMethod]
    public async Task LoginShouldSucceed()
    {
        AppDbContext appDbContext = _appDbContextCreator.CreateContext();
        User user = new User { Email = "test@gmail.com", FiatCurrencyType = appDbContext.FiatCurrencyTypes.First() };
        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();

        _passwordHasherMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        _jwtHelperMock.Setup(x => x.CreateAccessToken(It.IsAny<User>())).Returns("token");
        _jwtHelperMock
            .Setup(x => x.CreateRefreshToken(It.IsAny<User>()))
            .Returns((User user) => (new RefreshToken { UserId = user.Id }, "refreshToken"));

        LoginHandler handler = new LoginHandler(
            _passwordHasherMock.Object,
            _mapper,
            _jwtHelperMock.Object,
            appDbContext
        );
        LoginRequest request = new LoginRequest { Email = user.Email, Password = "12345" };

        (LoginResult result, string nonHashedRefreshToken) = await handler.Handle(request);
        Assert.AreEqual(user.Id, result.User.Id);
    }
}
