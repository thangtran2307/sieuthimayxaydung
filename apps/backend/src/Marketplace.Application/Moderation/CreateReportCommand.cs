using Marketplace.Application.Catalog;
using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.Moderation;
using Mediator;

namespace Marketplace.Application.Moderation;

/// <summary>Records a report against an existing listing for later admin review.</summary>
public sealed record CreateReportCommand(Guid ListingId, CreateReportRequest Body) : ICommand;

public sealed class CreateReportCommandHandler(
    IListingQueries listingQueries,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<CreateReportCommand>
{
    public async ValueTask<Unit> Handle(CreateReportCommand command, CancellationToken cancellationToken)
    {
        var contact = await listingQueries.GetListingContactAsync(command.ListingId, cancellationToken)
            ?? throw new NotFoundException("Listing not found", ErrorCodes.ListingNotFound);

        var body = command.Body;
        var report = Report.Create(
            contact.ListingId,
            body.Reason,
            clock.UtcNow,
            body.Details,
            body.ReporterContact);

        await unitOfWork.ReportRepository.AddAsync(report, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
