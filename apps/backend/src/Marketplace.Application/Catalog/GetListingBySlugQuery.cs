using Marketplace.Application.Common.Persistence;
using Mediator;

namespace Marketplace.Application.Catalog;

/// <summary>Gets a public (ACTIVE) listing by slug and records a view. 404 if not public.</summary>
public sealed record GetListingBySlugQuery(string Slug) : IQuery<ListingDetailDto>;

public sealed class GetListingBySlugQueryHandler(
    IListingQueries listingQueries,
    IUnitOfWork unitOfWork) : IQueryHandler<GetListingBySlugQuery, ListingDetailDto>
{
    public async ValueTask<ListingDetailDto> Handle(
        GetListingBySlugQuery query,
        CancellationToken cancellationToken)
    {
        var listing = await listingQueries.GetBySlugAsync(query.Slug, cancellationToken)
            ?? throw new NotFoundException("Listing not found", ErrorCodes.ListingNotFound);

        // Record the view on the write side (domain behavior + unit of work), not via raw SQL.
        var aggregate = await unitOfWork.ListingRepository.GetByIdAsync(listing.Id, cancellationToken);
        if (aggregate is null)
        {
            return listing with { ViewCount = listing.ViewCount + 1 };
        }

        aggregate.RegisterView();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Reflect the view we just counted without an extra round-trip.
        return listing with { ViewCount = listing.ViewCount + 1 };
    }
}
