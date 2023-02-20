using System.Net;
using Api.Common.Attributes;
using Api.Common.Exceptions;
using Api.Database;
using Api.Domain.Models;
using Api.Services;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Users.Common.Features;

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmedPassword { get; set; } = string.Empty;
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.ConfirmedPassword).Equal(x => x.Password);
    }
}

public class CreateUserResult
{
    public UserDTO User { get; set; } = null!;
    public string AccessToken { get; set; } = String.Empty;
}

[Inject]
public class CreateUserHandler
{
    IPasswordHasher _passwordHasher;
    IMapper _mapper;
    IJwtHelper _jwtHelper;
    AppDbContext _appDbContext;

    public CreateUserHandler(
        IPasswordHasher passwordHasher,
        IMapper mapper,
        IJwtHelper jwtHelper,
        AppDbContext appDbContext
    )
    {
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _jwtHelper = jwtHelper;
        _appDbContext = appDbContext;
    }

    public async Task<(CreateUserResult createUserResult, string nonHashedRefreshToken)> Handle(
        CreateUserRequest request
    )
    {
        bool emailIsTaken = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email) != null;
        if (emailIsTaken)
        {
            throw new ApiException(HttpStatusCode.Conflict, "Email is already taken");
        }

        User user = await CreateUser(request);

        string accessToken = _jwtHelper.CreateAccessToken(user);

        (RefreshToken refreshToken, string nonHashedRefreshToken) = _jwtHelper.CreateRefreshToken(user);
        _appDbContext.RefreshTokens.Add(refreshToken);
        await _appDbContext.SaveChangesAsync();

        CreateUserResult createUserResult = new CreateUserResult
        {
            User = _mapper.Map<UserDTO>(user),
            AccessToken = accessToken
        };

        return (createUserResult, nonHashedRefreshToken);
    }

    private async Task<User> CreateUser(CreateUserRequest request)
    {
        User user = new User { Email = request.Email, Password = _passwordHasher.HashPassword(request.Password) };
        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();
        return user;
    }
}
