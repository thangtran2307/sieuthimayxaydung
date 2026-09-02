using System.Security.Claims;
using Marketplace.Application.Common.Auth;

namespace Marketplace.Api.Security;

/// <summary>
/// Reads the authenticated caller from the current request's validated JWT claims
/// (<c>sub</c> = user id, role claim = role). Scoped: one instance per request.
/// </summary>
public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId =>
        Guid.TryParse(Principal?.FindFirstValue("sub"), out var id) ? id : null;

    public UserRole? Role =>
        Enum.TryParse<UserRole>(Principal?.FindFirstValue(ClaimTypes.Role), out var role) ? role : null;
}
