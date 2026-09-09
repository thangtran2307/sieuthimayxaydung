using FluentValidation;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.ModerationDecisions;
using Mediator;

namespace Marketplace.Application.Moderation;

/// <summary>Applies one moderation action to several listings at once (FR-022).</summary>
public sealed record BulkModerateRequest(
    IReadOnlyList<Guid> ListingIds,
    ModerationAction Action,
    string Reason);

public sealed class BulkModerateRequestValidator : AbstractValidator<BulkModerateRequest>
{
    public BulkModerateRequestValidator()
    {
        RuleFor(x => x.Action).IsInEnum();
        RuleFor(x => x.ListingIds).NotEmpty();
        RuleForEach(x => x.ListingIds).NotEmpty();
    }
}

public sealed record BulkModerateListingsCommand(BulkModerateRequest Body)
    : ICommand<BulkModerationResultDto>;

public sealed class BulkModerateListingsCommandHandler(
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<BulkModerateListingsCommand, BulkModerationResultDto>
{
    public async ValueTask<BulkModerationResultDto> Handle(
        BulkModerateListingsCommand command,
        CancellationToken cancellationToken)
    {
        var adminId = currentUser.RequireUserId();
        var body = command.Body;
        var ids = body.ListingIds.Distinct().ToList();

        // Load every requested listing in one query (WHERE id = ANY(...)), then work in memory.
        var listings = await unitOfWork.ListingRepository.ListAsync(
            x => ids.Contains(x.Id),
            cancellationToken);

        // Ids with no matching row are skipped rather than aborting the batch.
        int skipped = ids.Count - listings.Count;
        int succeeded = 0;

        foreach (var listing in listings)
        {
            try
            {
                // Skip listings in an incompatible state (e.g. approving one no longer pending).
                ModerationOps.Apply(listing, body.Action, clock.UtcNow);
            }
            catch (DomainRuleException)
            {
                skipped++;
                continue;
            }

            // Listings are tracked (loaded via ListAsync); the status change is persisted on save.
            await unitOfWork.ModerationDecisionRepository.AddAsync(
                ModerationDecision.Create(listing.Id, adminId, body.Action, clock.UtcNow, body.Reason),
                cancellationToken);
            succeeded++;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new BulkModerationResultDto(succeeded, skipped);
    }
}
