using Microsoft.AspNetCore.Http;
using Midiot.BL.Interfaces.User;
using Midiot.Common;

namespace Midiot.BL.Services.User;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = Guid.Parse(httpContext.User.FindFirst(CustomClaimTypes.UserId).Value);
        return userId;
    }
}
