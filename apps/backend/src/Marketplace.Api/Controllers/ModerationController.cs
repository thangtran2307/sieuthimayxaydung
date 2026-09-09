using Asp.Versioning;
using Marketplace.Application.Moderation;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

/// <summary>
/// Admin moderation endpoints (User Story 3). Restricted to administrators via the "Admin" policy
/// (FR-023); every action is recorded as a moderation decision for audit (FR-030).
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("moderation")]
[Authorize(Policy = "Admin")]
public sealed class ModerationController(IMediator mediator) : ControllerBase
{
    /// <summary>The pending-listing review queue (FR-019).</summary>
    [HttpGet("queue")]
    public async Task<IReadOnlyList<ModerationQueueItemDto>> Queue(CancellationToken cancellationToken) =>
        await mediator.Send(new GetModerationQueueQuery(), cancellationToken);

    /// <summary>Open reports awaiting resolution (FR-021).</summary>
    [HttpGet("reports")]
    public async Task<IReadOnlyList<ReportDto>> Reports(CancellationToken cancellationToken) =>
        await mediator.Send(new GetReportsQuery(), cancellationToken);

    /// <summary>Approve a pending listing, making it public (FR-020).</summary>
    [HttpPost("listings/{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromBody] ModerateListingRequest body,
        CancellationToken cancellationToken) =>
        await Moderate(id, ModerationAction.APPROVED, body, cancellationToken);

    /// <summary>Reject a pending listing (FR-020).</summary>
    [HttpPost("listings/{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] ModerateListingRequest body,
        CancellationToken cancellationToken) =>
        await Moderate(id, ModerationAction.REJECTED, body, cancellationToken);

    /// <summary>Remove a listing for a policy violation (FR-020).</summary>
    [HttpPost("listings/{id:guid}/remove")]
    public async Task<IActionResult> Remove(
        Guid id,
        [FromBody] ModerateListingRequest body,
        CancellationToken cancellationToken) =>
        await Moderate(id, ModerationAction.REMOVED, body, cancellationToken);

    /// <summary>Apply one action to several listings at once (FR-022).</summary>
    [HttpPost("listings/bulk")]
    public async Task<BulkModerationResultDto> Bulk(
        [FromBody] BulkModerateRequest body,
        CancellationToken cancellationToken) =>
        await mediator.Send(new BulkModerateListingsCommand(body), cancellationToken);

    /// <summary>Resolve a report by removing the listing or clearing the report (FR-021).</summary>
    [HttpPost("reports/{id:guid}/resolve")]
    public async Task<IActionResult> ResolveReport(
        Guid id,
        [FromBody] ResolveReportRequest body,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new ResolveReportCommand(id, body), cancellationToken);
        return NoContent();
    }

    private async Task<IActionResult> Moderate(
        Guid id,
        ModerationAction action,
        ModerateListingRequest body,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new ModerateListingCommand(id, action, body?.Reason), cancellationToken);
        return NoContent();
    }
}
