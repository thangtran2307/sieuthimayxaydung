using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Marketplace.Api.Swagger;

/// <summary>Well-known Swagger security scheme ids (the "Authorize" entries in the UI).</summary>
internal static class SwaggerSecuritySchemes
{
    public const string Bearer = "Bearer";
    public const string ApiKey = "ApiKey";
}

/// <summary>Registers one Swagger document per discovered API version (e.g. v1, v2) + auth schemes.</summary>
public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Title = "Marketplace API",
                    Version = description.ApiVersion.ToString(),
                    Description = description.IsDeprecated ? "This API version has been deprecated." : null
                });
        }

        // JWT access token — sent as `Authorization: Bearer <token>`. The API also accepts it from the
        // httpOnly `access_token` cookie in the browser; the header path is for Swagger/tools.
        options.AddSecurityDefinition(SwaggerSecuritySchemes.Bearer, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Paste a JWT access token (no 'Bearer ' prefix needed)."
        });

        // Internal API key — sent as the `X-Api-Key` header (used by /internal endpoints).
        options.AddSecurityDefinition(SwaggerSecuritySchemes.ApiKey, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "X-Api-Key",
            Description = "Internal API key for maintenance endpoints."
        });

        options.DocumentFilter<SecurityRequirementsDocumentFilter>();
    }
}
