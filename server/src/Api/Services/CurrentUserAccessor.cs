using System.Net;
using System.Security.Claims;
using Api.Common.Exceptions;

namespace Api.Services;

public interface ICurrentUserAccessor
{
    int GetCurrentUserId();
}

public class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetCurrentUserId()
    {
        string? userId = _httpContextAccessor
            ?.HttpContext?.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)
            .Value;
        if (userId is null)
        {
            throw new ApiException(HttpStatusCode.InternalServerError);
        }
        return Convert.ToInt32(userId);
    }
}
