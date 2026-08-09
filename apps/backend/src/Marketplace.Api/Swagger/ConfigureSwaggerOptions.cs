using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Marketplace.Api.Swagger;

/// <summary>Registers one Swagger document per discovered API version (e.g. v1, v2).</summary>
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
                    Description = description.IsDeprecated ? "This API version has been deprecated." : null,
                });
        }
    }
}
