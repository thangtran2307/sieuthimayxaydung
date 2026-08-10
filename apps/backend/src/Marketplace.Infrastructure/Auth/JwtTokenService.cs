using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Marketplace.Application.Common.Auth;
using Marketplace.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Infrastructure.Auth;

/// <summary>Issues and validates HS256 JWTs (Constitution VI). Tokens are carried in httpOnly cookies.</summary>
public sealed class JwtTokenService(IOptions<JwtOptions> options, IClock clock) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public TokenPair Issue(AuthPrincipal principal)
    {
        string access = CreateToken(
            principal,
            _options.AccessSecret,
            TimeSpan.FromMinutes(_options.AccessTtlMinutes));
        string refresh = CreateToken(
            principal,
            _options.RefreshSecret,
            TimeSpan.FromDays(_options.RefreshTtlDays));
        return new TokenPair(access, refresh);
    }

    public AuthPrincipal ValidateRefreshToken(string refreshToken)
    {
        var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
        try
        {
            var principal = handler.ValidateToken(
                refreshToken,
                new TokenValidationParameters
                {
                    ValidIssuer = _options.Issuer,
                    ValidAudience = _options.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_options.RefreshSecret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                },
                out _);

            string id = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            string email = principal.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty;
            string role = principal.FindFirstValue(ClaimTypes.Role) ?? nameof(UserRole.SELLER);

            if (id is null || !Guid.TryParse(id, out var userId))
            {
                return null;
            }

            return new AuthPrincipal(userId, Enum.Parse<UserRole>(role), email);
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException or FormatException)
        {
            return null;
        }
    }

    private string CreateToken(AuthPrincipal principal, string secret, TimeSpan lifetime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = clock.UtcNow;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, principal.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, principal.Email),
            new Claim(ClaimTypes.Role, principal.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            notBefore: now,
            expires: now.Add(lifetime),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
