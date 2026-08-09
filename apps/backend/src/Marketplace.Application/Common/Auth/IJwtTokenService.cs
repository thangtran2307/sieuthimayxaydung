namespace Marketplace.Application.Common.Auth;

/// <summary>The authenticated principal encoded in a JWT.</summary>
public sealed record AuthPrincipal(Guid UserId, UserRole Role, string Email);

/// <summary>An issued access + refresh token pair.</summary>
public sealed record TokenPair(string AccessToken, string RefreshToken);

/// <summary>Issues and validates JWTs (implemented in Infrastructure).</summary>
public interface IJwtTokenService
{
    TokenPair Issue(AuthPrincipal principal);

    AuthPrincipal ValidateRefreshToken(string refreshToken);
}
