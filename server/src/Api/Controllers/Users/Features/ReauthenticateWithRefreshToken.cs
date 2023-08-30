using System.Net;
using Api.Common.Attributes;
using Api.Common.Exceptions;
using Api.Database;
using Api.Domain.Models;
using Api.Common.ExtensionMethods;
using Api.Utils;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Users.Common.Features;

public class ReauthenticateWithRefreshTokenResult
{
    public int UserId { get; set; }
    public string AccessToken { get; set; } = String.Empty;
}

[Inject]
public class ReauthenticateWithRefreshTokenHandler
{
    IJwtHelper _jwtHelper;
    AppDbContext _appDbContext;

    public ReauthenticateWithRefreshTokenHandler(IJwtHelper jwtHelper, AppDbContext appDbContext)
    {
        _jwtHelper = jwtHelper;
        _appDbContext = appDbContext;
    }

    public async Task<(
        ReauthenticateWithRefreshTokenResult reauthenticateWithRefreshTokenResult,
        string nonHashedRefreshToken
    )> Handle(string nonHashedRefreshToken)
    {
        string hashedRefreshToken = nonHashedRefreshToken.ToSHA512();

        RefreshToken? currentRefreshToken = await _appDbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(X => X.Token == hashedRefreshToken);
        if (currentRefreshToken is null)
        {
            throw new ApiException(HttpStatusCode.Unauthorized);
        }

        if (currentRefreshToken.Expires < DateTime.UtcNow)
        {
            throw new ApiException(HttpStatusCode.Unauthorized);
        }

        User user = currentRefreshToken.User;
        string accessToken = _jwtHelper.CreateAccessToken(user);
        (RefreshToken newRefreshToken, string newNonHashedRefreshToken) = _jwtHelper.CreateRefreshToken(user);

        _appDbContext.RefreshTokens.Add(newRefreshToken);
        _appDbContext.RefreshTokens.Remove(currentRefreshToken);
        await _appDbContext.SaveChangesAsync();
        await DeleteExpiredRefreshTokens(user);

        ReauthenticateWithRefreshTokenResult reauthenticateWithRefreshTokenResult =
            new ReauthenticateWithRefreshTokenResult { UserId = user.Id, AccessToken = accessToken };

        return (reauthenticateWithRefreshTokenResult, newNonHashedRefreshToken);
    }

    public async Task DeleteExpiredRefreshTokens(User user)
    {
        await _appDbContext.RefreshTokens
            .Where(x => x.Id == user.Id && x.Expires <= DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }
}
