using System.Security.Cryptography;
using System.Text;
using Marketplace.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Marketplace.Api.Security;

/// <summary>
/// Requires a valid internal API key (sent in the <c>X-Api-Key</c> header) on the decorated
/// controller/action. Reusable for any internal/maintenance endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ApiKeyAuthAttribute() : TypeFilterAttribute(typeof(ApiKeyAuthorizationFilter));

/// <summary>
/// Validates the <c>X-Api-Key</c> header against the configured internal key using a constant-time
/// comparison. Rejects with 401 when the key is missing, wrong, or not configured (fail closed).
/// </summary>
internal sealed class ApiKeyAuthorizationFilter(IOptions<InternalOptions> options) : IAsyncAuthorizationFilter
{
    private const string _headerName = "X-Api-Key";

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        string configuredKey = options.Value.ApiKey;
        string providedKey = context.HttpContext.Request.Headers[_headerName].ToString();

        if (string.IsNullOrEmpty(configuredKey) || !MatchesConfiguredKey(configuredKey, providedKey))
        {
            context.Result = new ObjectResult(new
            {
                error = new { code = ErrorCodes.Unauthenticated, message = "Invalid or missing API key." },
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized,
            };
        }

        return Task.CompletedTask;
    }

    private static bool MatchesConfiguredKey(string configuredKey, string providedKey)
    {
        if (string.IsNullOrEmpty(providedKey))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(configuredKey),
            Encoding.UTF8.GetBytes(providedKey));
    }
}
