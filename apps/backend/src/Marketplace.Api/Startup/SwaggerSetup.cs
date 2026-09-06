using Asp.Versioning.ApiExplorer;

namespace Marketplace.Api.Startup;

/// <summary>Swagger middleware wiring — one UI document per discovered API version.</summary>
internal static class SwaggerSetup
{
    public static void UseVersionedSwaggerUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (string groupName in provider.ApiVersionDescriptions.Select(d => d.GroupName))
            {
                options.SwaggerEndpoint(
                    $"/swagger/{groupName}/swagger.json",
                    groupName.ToUpperInvariant());
            }

            // Send cookies with "Try it out" requests so cookie auth works: call /auth/login, then the
            // browser stores the httpOnly access_token cookie and attaches it to protected calls
            // automatically (no need to paste a bearer token). Same-origin + SameSite=Lax allow this.
            options.UseRequestInterceptor("(request) => { request.credentials = 'include'; return request; }");
        });
    }
}
