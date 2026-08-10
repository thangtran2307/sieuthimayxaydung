using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.Catalog;
using Marketplace.Domain.Moderation;
using InquiryEntity = Marketplace.Domain.Inquiry.Inquiry;

namespace Marketplace.Infrastructure.Persistence;

/// <summary>
/// EF Core unit of work — exposes each aggregate's write repository (lazily created over the shared
/// DbContext) and commits all tracked changes in one transaction on the write connection.
/// </summary>
public sealed class EfUnitOfWork(MarketplaceDbContext dbContext) : IUnitOfWork
{
    private IRepository<Listing> _listingRepository;
    private IRepository<InquiryEntity> _inquiryRepository;
    private IRepository<Report> _reportRepository;

    public IRepository<Listing> ListingRepository =>
        _listingRepository ??= new EfRepository<Listing>(dbContext);

    public IRepository<InquiryEntity> InquiryRepository =>
        _inquiryRepository ??= new EfRepository<InquiryEntity>(dbContext);

    public IRepository<Report> ReportRepository =>
        _reportRepository ??= new EfRepository<Report>(dbContext);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
