using Asp.Versioning;
using Marketplace.Application.Listings;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

/// <summary>
/// Seller listing management (User Story 2). Requires an authenticated account (FR-013); every action
/// operates only on the caller's own listings (ownership enforced in the handlers, FR-017).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("seller/listings")]
[Authorize]
public sealed class SellerListingsController(IMediator mediator) : ControllerBase
{
    /// <summary>Creates a new listing (starts pending review, FR-016).</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateListingRequest body,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateListingCommand(body), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Lists the current seller's own listings with status + view counts (FR-018).</summary>
    [HttpGet]
    public async Task<IReadOnlyList<SellerListingDto>> Mine(CancellationToken cancellationToken) =>
        await mediator.Send(new GetMyListingsQuery(), cancellationToken);

    /// <summary>Edits one of the current seller's listings.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateListingRequest body,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateListingCommand(id, body), cancellationToken);
        return NoContent();
    }

    /// <summary>Marks one of the current seller's listings as sold.</summary>
    [HttpPost("{id:guid}/sold")]
    public async Task<IActionResult> MarkSold(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new MarkListingSoldCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Removes one of the current seller's listings (soft delete).</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteListingCommand(id), cancellationToken);
        return NoContent();
    }
}
