using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Api.Domain.Models;
using Api.Common.ExtensionMethods;
using Microsoft.IdentityModel.Tokens;

namespace Api.Utils;

public interface IJwtHelper
{
    string CreateAccessToken(User user);
    (RefreshToken refreshToken, string nonHashedRefreshToken) CreateRefreshToken(User user);
}

public class JwtHelper : IJwtHelper
{
    public string CreateAccessToken(User user)
    {
        string jwtKey = DotNetEnv.Env.GetString("JWT_SECRET") ?? "";
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        SigningCredentials signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha512Signature
        );

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(15),
            Subject = new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), }),
            SigningCredentials = signingCredentials
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public (RefreshToken refreshToken, string nonHashedRefreshToken) CreateRefreshToken(User user)
    {
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        string hashedToken = token.ToSHA512();

        RefreshToken refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = hashedToken,
            Expires = DateTime.UtcNow.AddMonths(1)
        };

        return (refreshToken, token);
    }
}
