using FluentValidation;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.ModerationDecisions;
using Mediator;

namespace Marketplace.Application.Moderation;

/// <summary>An admin's resolution of a report — remove the listing or clear the report (FR-021).</summary>
public sealed record ResolveReportRequest(ReportResolution Resolution, string Reason);

public sealed class ResolveReportRequestValidator : AbstractValidator<ResolveReportRequest>
{
    public ResolveReportRequestValidator()
    {
        RuleFor(x => x.Resolution).IsInEnum();
        RuleFor(x => x.Reason).MaximumLength(2000);
    }
}

public sealed record ResolveReportCommand(Guid ReportId, ResolveReportRequest Body) : ICommand;

public sealed class ResolveReportCommandHandler(
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<ResolveReportCommand>
{
    public async ValueTask<Unit> Handle(ResolveReportCommand command, CancellationToken cancellationToken)
    {
        var adminId = currentUser.RequireUserId();
        var now = clock.UtcNow;

        var report = await unitOfWork.ReportRepository.FirstOrDefaultAsync(
            x => x.Id == command.ReportId,
            cancellationToken)
            ?? throw new NotFoundException("Report not found", ErrorCodes.NotFound);

        if (command.Body.Resolution == ReportResolution.REMOVE)
        {
            var listing = await unitOfWork.ListingRepository.FirstOrDefaultAsync(
                x => x.Id == report.ListingId,
                cancellationToken);

            if (listing is not null && listing.Status != ListingStatus.REMOVED)
            {
                listing.Remove(now);
                await unitOfWork.ModerationDecisionRepository.AddAsync(
                    ModerationDecision.Create(listing.Id, adminId, ModerationAction.REMOVED, now, command.Body.Reason),
                    cancellationToken);
            }

            report.Resolve(adminId, ReportStatus.RESOLVED_REMOVED, now);
        }
        else
        {
            report.Resolve(adminId, ReportStatus.RESOLVED_CLEARED, now);
        }

        // Both the report and (if removed) the listing are tracked, so their mutations persist on
        // save via the change tracker — only the new ModerationDecision is explicitly added.
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
