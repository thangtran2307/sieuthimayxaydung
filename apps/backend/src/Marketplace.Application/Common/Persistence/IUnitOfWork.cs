using Marketplace.Domain.Inquiries;
using Marketplace.Domain.Listings;
using Marketplace.Domain.Reports;
using Marketplace.Domain.Samples;

namespace Marketplace.Application.Common.Persistence;

/// <summary>
/// Unit of Work — the single write-side entry point. Exposes each aggregate's write repository as a
/// named property (so handlers inject only this, not every repository) and commits all tracked
/// changes in one transaction. Command handlers call <see cref="SaveChangesAsync"/> once at the end
/// of a use-case. Reads use the Dapper query classes on the read connection (CQRS split).
/// </summary>
public interface IUnitOfWork
{
    IListingRepository ListingRepository { get; }

    IInquiryRepository InquiryRepository { get; }

    IReportRepository ReportRepository { get; }

    ISampleRepository SampleRepository { get; }

    /// <summary>Persists all tracked changes and returns the number of affected rows.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists all tracked changes, then publishes any domain events raised by aggregates during
    /// the use case. Events are dispatched after the commit so side-effects never run on a
    /// rolled-back write.
    /// </summary>
    Task<int> SaveChangesAndPublishEventsAsync(CancellationToken cancellationToken = default);
}
