using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.Listings;
using Marketplace.Domain.ModerationDecisions;
using Mediator;

namespace Marketplace.Application.Moderation;

/// <summary>Optional free-text reason recorded with a moderation decision.</summary>
public sealed record ModerateListingRequest(string Reason);

/// <summary>Applies an admin moderation action (approve/reject/remove) to a single listing (FR-020).</summary>
public sealed record ModerateListingCommand(Guid ListingId, ModerationAction Action, string Reason)
    : ICommand;

public sealed class ModerateListingCommandHandler(
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<ModerateListingCommand>
{
    public async ValueTask<Unit> Handle(ModerateListingCommand command, CancellationToken cancellationToken)
    {
        var adminId = currentUser.RequireUserId();

        var listing = await unitOfWork.ListingRepository.FirstOrDefaultAsync(
            x => x.Id == command.ListingId,
            cancellationToken)
            ?? throw new NotFoundException("Listing not found", ErrorCodes.ListingNotFound);

        // The listing is tracked (loaded via the repository), so the change tracker persists the
        // status change on save — only the new ModerationDecision needs to be added.
        ModerationOps.Apply(listing, command.Action, clock.UtcNow);

        await unitOfWork.ModerationDecisionRepository.AddAsync(
            ModerationDecision.Create(listing.Id, adminId, command.Action, clock.UtcNow, command.Reason),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

/// <summary>Shared listing state transitions for moderation, used by single and bulk actions.</summary>
internal static class ModerationOps
{
    public static void Apply(Listing listing, ModerationAction action, DateTime now)
    {
        switch (action)
        {
            case ModerationAction.APPROVED:
                listing.Approve(now);
                break;
            case ModerationAction.REJECTED:
                listing.Reject(now);
                break;
            case ModerationAction.REMOVED:
                listing.Remove(now);
                break;
            default:
                throw new DomainRuleException("Unknown moderation action.");
        }
    }
}
