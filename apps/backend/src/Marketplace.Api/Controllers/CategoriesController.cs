using Asp.Versioning;
using Marketplace.Application.Catalog;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

/// <summary>Public taxonomy endpoint (categories + subcategories).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("categories")]
public sealed class CategoriesController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists the full category taxonomy.</summary>
    [HttpGet]
    public async Task<IReadOnlyList<CategoryDto>> GetAll(CancellationToken cancellationToken) =>
        await mediator.Send(new GetCategoriesQuery(), cancellationToken);
}
