using Marketplace.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Marketplace.Api.Swagger;

/// <summary>
/// Attaches a security requirement to each operation that needs one — the internal API key for
/// <c>[ApiKeyAuth]</c> endpoints and the JWT bearer for <c>[Authorize]</c> endpoints — so Swagger UI
/// sends the right credential. Runs as a document filter so the scheme references resolve against the
/// document's components. Public endpoints are left unlocked.
/// </summary>
internal sealed class SecurityRequirementsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var api in context.ApiDescriptions)
        {
            var metadata = api.ActionDescriptor.EndpointMetadata;
            bool requiresApiKey = metadata.OfType<ApiKeyAuthAttribute>().Any();
            bool requiresJwt = metadata.OfType<IAuthorizeData>().Any();

            if ((!requiresApiKey && !requiresJwt) || api.RelativePath is null || swaggerDoc.Paths is null)
            {
                continue;
            }

            // Match the OpenAPI path (keys start with '/') to this action's relative path.
            var pathItem = swaggerDoc.Paths
                .FirstOrDefault(p => string.Equals(
                    p.Key.TrimStart('/'),
                    api.RelativePath.TrimEnd('/'),
                    StringComparison.OrdinalIgnoreCase)).Value;

            var operation = pathItem?.Operations?
                .FirstOrDefault(kv => string.Equals(kv.Key.ToString(), api.HttpMethod, StringComparison.OrdinalIgnoreCase))
                .Value;
            if (operation is null)
            {
                continue;
            }

            operation.Security ??= [];
            if (requiresApiKey)
            {
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(SwaggerSecuritySchemes.ApiKey, swaggerDoc)] = [],
                });
            }

            if (requiresJwt)
            {
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(SwaggerSecuritySchemes.Bearer, swaggerDoc)] = [],
                });
            }
        }
    }
}
