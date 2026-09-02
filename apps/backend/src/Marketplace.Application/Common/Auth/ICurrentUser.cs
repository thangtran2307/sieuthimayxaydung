namespace Marketplace.Application.Common.Auth;

/// <summary>
/// The authenticated caller for the current request, read from the validated JWT claims. Implemented
/// in the API layer (over <c>HttpContext</c>); handlers inject this instead of touching HTTP.
/// </summary>
public interface ICurrentUser
{
    /// <summary>The authenticated user's id, or null when the request is anonymous.</summary>
    Guid? UserId { get; }

    /// <summary>The authenticated user's role, or null when the request is anonymous.</summary>
    UserRole? Role { get; }

    /// <summary>True when the request carries a valid authenticated identity.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Returns the authenticated user id or throws <see cref="UnauthenticatedException"/>.</summary>
    Guid RequireUserId() =>
        UserId ?? throw new UnauthenticatedException();
}
