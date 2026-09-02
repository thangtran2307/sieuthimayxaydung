using Marketplace.Application.Common.Auth;
using Mediator;

namespace Marketplace.Application.Listings;

/// <summary>Returns the authenticated seller's own listings for their dashboard (FR-018).</summary>
public sealed record GetMyListingsQuery : IQuery<IReadOnlyList<SellerListingDto>>;

public sealed class GetMyListingsQueryHandler(
    ICurrentUser currentUser,
    IListingQueries listingQueries) : IQueryHandler<GetMyListingsQuery, IReadOnlyList<SellerListingDto>>
{
    public async ValueTask<IReadOnlyList<SellerListingDto>> Handle(
        GetMyListingsQuery query,
        CancellationToken cancellationToken)
    {
        var sellerId = currentUser.RequireUserId();
        return await listingQueries.GetSellerListingsAsync(sellerId, cancellationToken);
    }
}
