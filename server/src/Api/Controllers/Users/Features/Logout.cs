using Api.Common.Attributes;
using Api.Common.ExtensionMethods;
using Api.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers.Users.Common.Features;

[Inject]
public class LogoutHandler
{
    AppDbContext _appDbContext;

    public LogoutHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task Handle(string nonHashedRefreshToken)
    {
        string hashedRefreshToken = nonHashedRefreshToken.ToSHA512();
        await _appDbContext.RefreshTokens.Where(X => X.Token == hashedRefreshToken).ExecuteDeleteAsync();
    }
}
