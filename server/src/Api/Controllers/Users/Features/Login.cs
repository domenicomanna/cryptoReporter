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

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginResult
{
    public int UserId { get; set; }
    public string AccessToken { get; set; } = String.Empty;
}

[Inject]
public class LoginHandler
{
    IPasswordHasher _passwordHasher;
    IMapper _mapper;
    IJwtHelper _jwtHelper;
    AppDbContext _appDbContext;

    public LoginHandler(IPasswordHasher passwordHasher, IMapper mapper, IJwtHelper jwtHelper, AppDbContext appDbContext)
    {
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _jwtHelper = jwtHelper;
        _appDbContext = appDbContext;
    }

    public async Task<(LoginResult loginResult, string nonHashedRefreshToken)> Handle(LoginRequest request)
    {
        User? user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null)
        {
            throw new ApiException(HttpStatusCode.Unauthorized, "Invalid email or password");
        }

        bool passwordIsValid = _passwordHasher.VerifyPassword(request.Password, user.Password);
        if (!passwordIsValid)
        {
            throw new ApiException(HttpStatusCode.Unauthorized, "Invalid email or password");
        }

        string accessToken = _jwtHelper.CreateAccessToken(user);
        (RefreshToken refreshToken, string nonHashedRefreshToken) = _jwtHelper.CreateRefreshToken(user);

        _appDbContext.RefreshTokens.Add(refreshToken);
        await _appDbContext.SaveChangesAsync();

        LoginResult loginResult = new LoginResult { UserId = user.Id, AccessToken = accessToken };

        return (loginResult, nonHashedRefreshToken);
    }
}
