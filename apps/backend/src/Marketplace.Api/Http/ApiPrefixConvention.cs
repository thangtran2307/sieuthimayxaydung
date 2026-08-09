using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Marketplace.Api.Http;

/// <summary>Prefixes all controller routes with `api` (e.g. `[Route("health")]` → `/api/health`).</summary>
public sealed class ApiPrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _prefix = new(new Microsoft.AspNetCore.Mvc.RouteAttribute("api"));

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
