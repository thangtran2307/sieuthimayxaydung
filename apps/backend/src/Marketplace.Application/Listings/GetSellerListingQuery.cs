using Marketplace.Application.Common.Auth;
using Mediator;

namespace Marketplace.Application.Listings;

/// <summary>Loads one of the current seller's own listings to pre-fill the edit form (FR-017).</summary>
public sealed record GetSellerListingQuery(Guid ListingId) : IQuery<SellerListingDetailDto>;

public sealed class GetSellerListingQueryHandler(
    ICurrentUser currentUser,
    IListingQueries listingQueries) : IQueryHandler<GetSellerListingQuery, SellerListingDetailDto>
{
    public async ValueTask<SellerListingDetailDto> Handle(
        GetSellerListingQuery query,
        CancellationToken cancellationToken)
    {
        var sellerId = currentUser.RequireUserId();

        // The query is owner-scoped, so a listing that is missing OR owned by someone else is a 404
        // (a read never reveals the existence of another seller's listing).
        return await listingQueries.GetSellerListingAsync(sellerId, query.ListingId, cancellationToken)
            ?? throw new NotFoundException("Listing not found", ErrorCodes.ListingNotFound);
    }
}
