using Api.Common.Exceptions;
using Api.Controllers.Users;
using Api.Controllers.Users.Common.Features;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using AutoMapper;
using Fixtures;
using FluentValidation.TestHelper;
using Moq;

namespace ApiTests.Controllers.Users.Features;

public class CreateUserTests : IClassFixture<DatabaseFixture>
{
    IMapper _mapper = null!;
    DatabaseFixture _databaseFixture;
    Mock<IPasswordHasher> _passwordHasherMock = null!;
    Mock<IJwtHelper> _jwtHelperMock = null!;

    public CreateUserTests(DatabaseFixture databaseFixture)
    {
        MapperConfiguration mapperConfiguration = new MapperConfiguration(opts =>
        {
            opts.AddProfile(new UsersMappingProfile());
        });
        _mapper = mapperConfiguration.CreateMapper();
        _databaseFixture = databaseFixture;
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtHelperMock = new Mock<IJwtHelper>();
    }

    [Fact]
    public void ThereShouldBeAValidationErrorIfTheCurrencyIsInvalid()
    {
        CreateUserRequestValidator validator = new CreateUserRequestValidator();
        CreateUserRequest request = new CreateUserRequest
        {
            Email = "test@gmail.com",
            Password = "test12345",
            ConfirmedPassword = "test12345",
            FiatCurrencyType = "xyz123"
        };
        var result = validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FiatCurrencyType);
    }

    [Fact]
    public async Task AnExceptionShouldBeThrownIfTheEmailIsTaken()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();
        User user = new User { Email = "test@gmail.com", FiatCurrencyType = appDbContext.FiatCurrencyTypes.First() };
        appDbContext.Users.Add(user);
        appDbContext.SaveChanges();

        CreateUserHandler handler = new CreateUserHandler(
            _passwordHasherMock.Object,
            _mapper,
            _jwtHelperMock.Object,
            appDbContext
        );
        CreateUserRequest request = new CreateUserRequest
        {
            Email = user.Email,
            Password = "12345",
            ConfirmedPassword = "12345",
            FiatCurrencyType = "USD"
        };

        await Assert.ThrowsAsync<ApiException>(async () => await handler.Handle(request));
    }

    [Fact]
    public async Task TheUserShouldBeCreatedSuccessfully()
    {
        AppDbContext appDbContext = await _databaseFixture.CreateContext();

        _jwtHelperMock.Setup(x => x.CreateAccessToken(It.IsAny<User>())).Returns("token");
        _jwtHelperMock
            .Setup(x => x.CreateRefreshToken(It.IsAny<User>()))
            .Returns((User user) => (new RefreshToken { UserId = user.Id }, "refreshToken"));

        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("test");

        CreateUserHandler handler = new CreateUserHandler(
            _passwordHasherMock.Object,
            _mapper,
            _jwtHelperMock.Object,
            appDbContext
        );
        CreateUserRequest request = new CreateUserRequest
        {
            Email = "test@gmail.com",
            Password = "test12345",
            ConfirmedPassword = "test12345",
            FiatCurrencyType = "USD"
        };

        (CreateUserResult result, string nonHashedRefreshToken) = await handler.Handle(request);
        Assert.Equal(request.Email, result.User.Email);
    }
}
