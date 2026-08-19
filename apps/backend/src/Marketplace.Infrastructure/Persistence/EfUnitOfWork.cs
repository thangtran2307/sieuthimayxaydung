using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.Inquiries;
using Marketplace.Domain.Listings;
using Marketplace.Domain.Reports;
using Marketplace.Infrastructure.Persistence.Repositories;
using Mediator;

namespace Marketplace.Infrastructure.Persistence;

/// <summary>
/// EF Core unit of work — exposes each aggregate's write repository (lazily created over the shared
/// DbContext) and commits all tracked changes in one transaction on the write connection.
/// </summary>
public sealed class EfUnitOfWork(MarketplaceDbContext dbContext, IPublisher publisher) : IUnitOfWork
{
    private IListingRepository _listingRepository;
    private IInquiryRepository _inquiryRepository;
    private IReportRepository _reportRepository;

    public IListingRepository ListingRepository =>
        _listingRepository ??= new ListingRepository(dbContext);

    public IInquiryRepository InquiryRepository =>
        _inquiryRepository ??= new InquiryRepository(dbContext);

    public IReportRepository ReportRepository =>
        _reportRepository ??= new ReportRepository(dbContext);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);

    public async Task<int> SaveChangesAndPublishEventsAsync(CancellationToken cancellationToken = default)
    {
        // Snapshot events before saving — domain methods may raise them during the use-case, not
        // during SaveChanges itself, so collecting here captures everything correctly.
        // Collect from all tracked entities (IAggregateRoot roots in practice, but Entity is the
        // common tracked base so we iterate that and filter by presence of events).
        var aggregates = dbContext.ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .ToList();

        // Publish first: if any handler throws, SaveChanges is never called and the DB stays clean.
        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                await publisher.Publish(domainEvent, cancellationToken);
            }

            aggregate.ClearDomainEvents();
        }

        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}
