using System.Security.Claims;
using System.Text;
using Marketplace.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Api.Startup;

/// <summary>JWT authentication (token read from the httpOnly <c>access_token</c> cookie) + policies.</summary>
internal static class AuthenticationSetup
{
    public static IServiceCollection AddCookieJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        string accessSecret = string.IsNullOrWhiteSpace(jwt.AccessSecret) ? new string('0', 32) : jwt.AccessSecret;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => ConfigureJwtBearer(options, jwt, accessSecret));

        services
            .AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireRole(nameof(UserRole.ADMIN)));

        return services;
    }

    private static void ConfigureJwtBearer(JwtBearerOptions options, JwtOptions jwt, string accessSecret)
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = "sub",
        };
        options.Events = new JwtBearerEvents { OnMessageReceived = ReadTokenFromCookie };
    }

    private static Task ReadTokenFromCookie(MessageReceivedContext context)
    {
        if (context.Request.Cookies.TryGetValue("access_token", out string token))
        {
            context.Token = token;
        }

        return Task.CompletedTask;
    }
}
