using Marketplace.Application.Catalog;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.Catalog;
using Marketplace.Domain.Common;
using Marketplace.Domain.Moderation;
using InquiryEntity = Marketplace.Domain.Inquiry.Inquiry;

namespace Marketplace.Application.Tests.TestDoubles;

/// <summary>Fixed-time clock for deterministic tests.</summary>
internal sealed class StubClock(DateTime now) : IClock
{
    public DateTime UtcNow { get; } = now;
}

/// <summary>In-memory read side capturing the criteria it was called with.</summary>
internal sealed class FakeListingQueries : IListingQueries
{
    public ListingSearchCriteria CapturedCriteria { get; private set; }

    public PagedResult<ListingSummaryDto> SearchResult { get; set; } =
        PagedResult<ListingSummaryDto>.Create([], 0, 1, 20);

    public ListingDetailDto DetailResult { get; set; }

    public ListingContact ContactResult { get; set; }

    public Task<PagedResult<ListingSummaryDto>> SearchAsync(
        ListingSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        CapturedCriteria = criteria;
        return Task.FromResult(SearchResult);
    }

    public Task<ListingDetailDto> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        Task.FromResult(DetailResult);

    public Task<ListingContact> GetListingContactAsync(Guid listingId, CancellationToken cancellationToken = default) =>
        Task.FromResult(ContactResult);
}

/// <summary>Generic in-memory write repository recording added entities and returning a preset root.</summary>
internal class FakeRepository<T> : IRepository<T>
    where T : Entity
{
    public List<T> Added { get; } = [];

    public T ToReturn { get; set; }

    public Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(ToReturn);

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        Added.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(T entity)
    {
    }

    public void Remove(T entity)
    {
    }
}

/// <summary>In-memory unit of work exposing the fake repositories and counting commits.</summary>
internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public FakeRepository<Listing> Listings { get; } = new();

    public FakeRepository<InquiryEntity> Inquiries { get; } = new();

    public FakeRepository<Report> Reports { get; } = new();

    public int SaveChangesCallCount { get; private set; }

    IRepository<Listing> IUnitOfWork.ListingRepository => Listings;

    IRepository<InquiryEntity> IUnitOfWork.InquiryRepository => Inquiries;

    IRepository<Report> IUnitOfWork.ReportRepository => Reports;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;
        return Task.FromResult(1);
    }
}
