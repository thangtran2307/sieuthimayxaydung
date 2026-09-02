using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Mediator;

namespace Marketplace.Application.Listings;

/// <summary>
/// Removes the current seller's listing (FR-017). Soft-delete: the row is retained (status REMOVED)
/// for audit and to preserve referencing reports/boosts (FR-030); it is excluded from public reads.
/// </summary>
public sealed record DeleteListingCommand(Guid ListingId) : ICommand;

public sealed class DeleteListingCommandHandler(
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<DeleteListingCommand>
{
    public async ValueTask<Unit> Handle(DeleteListingCommand command, CancellationToken cancellationToken)
    {
        var listing = await SellerListing.LoadOwnedAsync(
            unitOfWork,
            currentUser,
            command.ListingId,
            cancellationToken);

        listing.Remove(clock.UtcNow);

        unitOfWork.ListingRepository.Update(listing);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
