using Marketplace.Domain.Catalog;
using Marketplace.Domain.Moderation;
using InquiryEntity = Marketplace.Domain.Inquiry.Inquiry;

namespace Marketplace.Application.Common.Persistence;

/// <summary>
/// Unit of Work — the single write-side entry point. Exposes each aggregate's write repository as a
/// named property (so handlers inject only this, not every repository) and commits all tracked
/// changes in one transaction. Command handlers call <see cref="SaveChangesAsync"/> once at the end
/// of a use-case. Reads use the Dapper query classes on the read connection (CQRS split).
/// </summary>
public interface IUnitOfWork
{
    IRepository<Listing> ListingRepository { get; }

    IRepository<InquiryEntity> InquiryRepository { get; }

    IRepository<Report> ReportRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
