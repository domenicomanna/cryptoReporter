using Api.Controllers.Users.Common;
using Api.Controllers.Users.Common.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Users;

public class UsersController : BaseApiController
{
    private readonly string _refreshTokenCookieName = "refreshToken";

    [AllowAnonymous]
    [HttpPost("")]
    public async Task<CreateUserResult> CreateUser([FromServices] CreateUserHandler handler, CreateUserRequest request)
    {
        (CreateUserResult createUserResult, string refreshToken) = await handler.Handle(request);
        CookieOptions refreshTokenCookieOptions = GetRefreshTokenCookieOptions();
        Response.Cookies.Append(_refreshTokenCookieName, refreshToken, refreshTokenCookieOptions);
        return createUserResult;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<LoginResult> Login([FromServices] LoginHandler handler, LoginRequest request)
    {
        (LoginResult loginResult, string refreshToken) = await handler.Handle(request);
        CookieOptions refreshTokenCookieOptions = GetRefreshTokenCookieOptions();
        Response.Cookies.Append(_refreshTokenCookieName, refreshToken, refreshTokenCookieOptions);
        return loginResult;
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task Logout([FromServices] LogoutHandler handler)
    {
        string refreshTokenCookieValue = Request.Cookies[_refreshTokenCookieName] ?? "";
        await handler.Handle(refreshTokenCookieValue);
        Response.Cookies.Delete(_refreshTokenCookieName);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ReauthenticateWithRefreshTokenResult> ReauthenticateWithRefreshToken(
        [FromServices] ReauthenticateWithRefreshTokenHandler handler
    )
    {
        string refreshTokenCookieValue = Request.Cookies[_refreshTokenCookieName] ?? "";
        (ReauthenticateWithRefreshTokenResult result, string refreshToken) = await handler.Handle(
            refreshTokenCookieValue
        );
        CookieOptions refreshTokenCookieOptions = GetRefreshTokenCookieOptions();
        Response.Cookies.Append(_refreshTokenCookieName, refreshToken, refreshTokenCookieOptions);
        return result;
    }

    private CookieOptions GetRefreshTokenCookieOptions()
    {
        return new CookieOptions
        {
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7),
        };
    }

    [HttpGet("{userId}")]
    public async Task<UserDTO> GetUser([FromServices] GetUserHandler handler, int userId)
    {
        return await handler.Handle(userId);
    }
}
