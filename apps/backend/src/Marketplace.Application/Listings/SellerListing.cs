using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.Listings;

namespace Marketplace.Application.Listings;

/// <summary>Shared write-side helpers for seller-owned listing use cases.</summary>
internal static class SellerListing
{
    /// <summary>
    /// Loads a listing that the current user owns, or throws: <see cref="NotFoundException"/> when it
    /// does not exist, <see cref="ForbiddenException"/> when it belongs to another seller (FR-017).
    /// </summary>
    public static async Task<Listing> LoadOwnedAsync(
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        Guid listingId,
        CancellationToken cancellationToken)
    {
        var sellerId = currentUser.RequireUserId();

        var listing = await unitOfWork.ListingRepository.FirstOrDefaultAsync(
            x => x.Id == listingId,
            cancellationToken)
            ?? throw new NotFoundException("Listing not found", ErrorCodes.ListingNotFound);

        return listing.SellerId != sellerId
            ? throw new ForbiddenException("You do not own this listing.", ErrorCodes.NotListingOwner)
            : listing;
    }
}
