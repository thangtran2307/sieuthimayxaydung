using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Mediator;

namespace Marketplace.Application.Listings;

/// <summary>Marks the current seller's listing as sold (FR-017).</summary>
public sealed record MarkListingSoldCommand(Guid ListingId) : ICommand;

public sealed class MarkListingSoldCommandHandler(
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<MarkListingSoldCommand>
{
    public async ValueTask<Unit> Handle(MarkListingSoldCommand command, CancellationToken cancellationToken)
    {
        var listing = await SellerListing.LoadOwnedAsync(
            unitOfWork,
            currentUser,
            command.ListingId,
            cancellationToken);

        listing.MarkSold(clock.UtcNow);

        unitOfWork.ListingRepository.Update(listing);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
