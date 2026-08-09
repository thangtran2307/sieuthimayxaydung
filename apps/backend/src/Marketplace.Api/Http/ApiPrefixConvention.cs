using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Marketplace.Api.Http;

/// <summary>
/// Prefixes all controller routes with `api/v{version}` (e.g. `[Route("listings")]` →
/// `/api/v1/listings`). Controllers must declare `[ApiVersion("1.0")]`; the `apiVersion` route
/// constraint is registered by API versioning.
/// </summary>
public sealed class ApiPrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _prefix =
        new(new Microsoft.AspNetCore.Mvc.RouteAttribute("api/v{version:apiVersion}"));

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                selector.AttributeRouteModel = selector.AttributeRouteModel is null
                    ? _prefix
                    : AttributeRouteModel.CombineAttributeRouteModel(_prefix, selector.AttributeRouteModel);
            }
        }
    }
}
