using Marketplace.Application.Common.Models;

namespace Marketplace.Application.Catalog;

/// <summary>
/// Read side (Dapper) for listings — search, public detail, and contact lookup. Runs on the read
/// connection. Returns wire DTOs directly; domain entities never cross this boundary.
/// </summary>
public interface IListingQueries
{
    Task<PagedResult<ListingSummaryDto>> SearchAsync(
        ListingSearchCriteria criteria,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the public detail for an ACTIVE listing by slug, or null if not public.</summary>
    Task<ListingDetailDto> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Returns contact/status info for a listing regardless of status, or null if missing.</summary>
    Task<ListingContact> GetListingContactAsync(Guid listingId, CancellationToken cancellationToken = default);
}
