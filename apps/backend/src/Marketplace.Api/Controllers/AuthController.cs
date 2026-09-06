using Asp.Versioning;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Identities;
using Marketplace.Infrastructure.Configuration;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Marketplace.Api.Controllers;

/// <summary>
/// Authentication endpoints (User Story 2). Tokens are issued as httpOnly, Secure, SameSite cookies
/// (Constitution VI); the response body never contains the tokens, only the account summary.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("auth")]
public sealed class AuthController(IMediator mediator, IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private const string _accessCookie = "access_token";
    private const string _refreshCookie = "refresh_token";

    private readonly JwtOptions _jwt = jwtOptions.Value;

    /// <summary>Registers a new seller and starts an authenticated session.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterSellerRequest body,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RegisterSellerCommand(body), cancellationToken);
        SetAuthCookies(result.Tokens);
        return StatusCode(StatusCodes.Status201Created, result.User);
    }

    /// <summary>Signs in with email/password.</summary>
    [HttpPost("login")]
    public async Task<AuthUserDto> Login(
        [FromBody] LoginRequest body,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginCommand(body), cancellationToken);
        SetAuthCookies(result.Tokens);
        return result.User;
    }

    /// <summary>Returns the authenticated caller's account summary.</summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<AuthUserDto> Me(CancellationToken cancellationToken) =>
        await mediator.Send(new GetCurrentUserQuery(), cancellationToken);

    /// <summary>Rotates the session using the refresh-token cookie.</summary>
    [HttpPost("refresh")]
    public async Task<AuthUserDto> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(_refreshCookie, out string refreshToken)
            || string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthenticatedException("No active session.");
        }

        var result = await mediator.Send(new RefreshTokenCommand(refreshToken), cancellationToken);
        SetAuthCookies(result.Tokens);
        return result.User;
    }

    /// <summary>Ends the session by clearing the auth cookies.</summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(_accessCookie, BaseCookieOptions());
        Response.Cookies.Delete(_refreshCookie, BaseCookieOptions());
        return NoContent();
    }

    private void SetAuthCookies(TokenPair tokens)
    {
        Response.Cookies.Append(_accessCookie, tokens.AccessToken, CookieOptions(
            TimeSpan.FromMinutes(_jwt.AccessTtlMinutes)));
        Response.Cookies.Append(_refreshCookie, tokens.RefreshToken, CookieOptions(
            TimeSpan.FromDays(_jwt.RefreshTtlDays)));
    }

    private static CookieOptions CookieOptions(TimeSpan lifetime)
    {
        var options = BaseCookieOptions();
        options.MaxAge = lifetime;
        return options;
    }

    // httpOnly + Secure + SameSite (Constitution VI). The frontend and API share an origin behind the
    // reverse proxy in every deployed environment, so SameSite=Lax is sufficient and safest.
    private static CookieOptions BaseCookieOptions() => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Lax,
        Path = "/",
    };
}
