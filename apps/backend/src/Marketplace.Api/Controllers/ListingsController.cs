using Asp.Versioning;
using Marketplace.Application.Catalog;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Inquiry;
using Marketplace.Application.Moderation;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

/// <summary>Public listing discovery + contact endpoints (User Story 1).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("listings")]
public sealed class ListingsController(IMediator mediator) : ControllerBase
{
    /// <summary>Search / filter / sort ACTIVE listings; boosted listings are pinned first.</summary>
    [HttpGet]
    public async Task<PagedResult<ListingSummaryDto>> Search(
        [FromQuery] SearchListingsQuery query,
        CancellationToken cancellationToken) =>
        await mediator.Send(query, cancellationToken);

    /// <summary>Get a public listing by slug (records a view).</summary>
    [HttpGet("{slug}")]
    public async Task<ListingDetailDto> GetBySlug(string slug, CancellationToken cancellationToken) =>
        await mediator.Send(new GetListingBySlugQuery(slug), cancellationToken);

    /// <summary>Contact the seller (phone reveal or message). No account required.</summary>
    [HttpPost("{id:guid}/inquiries")]
    public async Task<IActionResult> CreateInquiry(
        Guid id,
        [FromBody] CreateInquiryRequest body,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateInquiryCommand(id, body), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Report a listing for admin review. No account required.</summary>
    [HttpPost("{id:guid}/reports")]
    public async Task<IActionResult> CreateReport(
        Guid id,
        [FromBody] CreateReportRequest body,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateReportCommand(id, body), cancellationToken);
        return StatusCode(StatusCodes.Status201Created);
    }
}
