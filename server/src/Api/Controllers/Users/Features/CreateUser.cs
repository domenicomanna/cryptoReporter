using System.Net;
using Api.Common.Attributes;
using Api.Common.Exceptions;
using Api.Common.ExtensionMethods;
using Api.Common.ExtensionMethods.ValidationRules;
using Api.Database;
using Api.Domain.Models;
using Api.Utils;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Users.Common.Features;

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmedPassword { get; set; } = string.Empty;
    public string FiatCurrencyType { get; set; } = string.Empty;
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        List<string> validFiatCurrencyTypes = Enum.GetValues<FiatCurrencyTypeId>()
            .Select(x => x.GetDescription().ToLower())
            .ToList();

        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).Password();
        RuleFor(x => x.ConfirmedPassword).Equal(x => x.Password);
        RuleFor(x => x.FiatCurrencyType)
            .NotEmpty()
            .Must(x => validFiatCurrencyTypes.Contains(x.ToLower()))
            .WithMessage($"Fiat currency must be one of {string.Join(", ", validFiatCurrencyTypes)}");
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
            AccessToken = accessToken,
        };

        return (createUserResult, nonHashedRefreshToken);
    }

    private async Task<User> CreateUser(CreateUserRequest request)
    {
        List<FiatCurrencyType> fiatCurrencyTypes = _appDbContext.FiatCurrencyTypes.ToList();
        User user = new User
        {
            Email = request.Email,
            Password = _passwordHasher.HashPassword(request.Password),
            FiatCurrencyType = fiatCurrencyTypes.First(fiatCurrencyType =>
                fiatCurrencyType.Name.ToLower() == request.FiatCurrencyType.ToLower()
            ),
        };
        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();
        return user;
    }
}
