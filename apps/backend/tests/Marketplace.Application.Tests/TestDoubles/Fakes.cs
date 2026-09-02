using System.Linq.Expressions;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Common.Persistence;
using Marketplace.Application.Listings;
using Marketplace.Domain.Common;
using Marketplace.Domain.Identities;
using Marketplace.Domain.Inquiries;
using Marketplace.Domain.Listings;
using Marketplace.Domain.Reports;

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

    public Guid CapturedSellerId { get; private set; }

    public IReadOnlyList<SellerListingDto> SellerListingsResult { get; set; } = [];

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

    public Task<IReadOnlyList<SellerListingDto>> GetSellerListingsAsync(
        Guid sellerId,
        CancellationToken cancellationToken = default)
    {
        CapturedSellerId = sellerId;
        return Task.FromResult(SellerListingsResult);
    }
}

/// <summary>Generic in-memory base for per-aggregate fake repositories.</summary>
internal class FakeRepository<T> : IRepository<T>
    where T : Entity, IAggregateRoot
{
    public List<T> Added { get; } = [];

    public T ToReturn { get; set; }

    public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        Task.FromResult(ToReturn);

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        Added.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(T entity) { }

    public void Remove(T entity) { }
}

internal sealed class FakeUserRepository : FakeRepository<User>, IUserRepository;

internal sealed class FakeListingRepository : FakeRepository<Listing>, IListingRepository;

internal sealed class FakeInquiryRepository : FakeRepository<Inquiry>, IInquiryRepository;

internal sealed class FakeReportRepository : FakeRepository<Report>, IReportRepository;

/// <summary>In-memory unit of work exposing the fake repositories and counting commits.</summary>
internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public FakeUserRepository Users { get; } = new();

    public FakeListingRepository Listings { get; } = new();

    public FakeInquiryRepository Inquiries { get; } = new();

    public FakeReportRepository Reports { get; } = new();

    public int SaveChangesCallCount { get; private set; }

    IUserRepository IUnitOfWork.UserRepository => Users;

    IListingRepository IUnitOfWork.ListingRepository => Listings;

    IInquiryRepository IUnitOfWork.InquiryRepository => Inquiries;

    IReportRepository IUnitOfWork.ReportRepository => Reports;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;
        return Task.FromResult(1);
    }

    public Task<int> SaveChangesAndPublishEventsAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;
        return Task.FromResult(1);
    }
}

/// <summary>Current-user stub; anonymous unless a user id is set.</summary>
internal sealed class FakeCurrentUser : ICurrentUser
{
    public Guid? UserId { get; set; }

    public UserRole? Role { get; set; }

    public bool IsAuthenticated => UserId is not null;
}

/// <summary>Reversible fake hasher — "hash" is the password prefixed, so Verify is exact-match.</summary>
internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string hash, string password) => hash == $"hashed:{password}";
}

/// <summary>Issues fixed tokens and returns a preset principal from refresh validation.</summary>
internal sealed class FakeJwtTokenService : IJwtTokenService
{
    public TokenPair TokenToIssue { get; set; } = new("access-token", "refresh-token");

    public AuthPrincipal RefreshResult { get; set; }

    public AuthPrincipal IssuedFor { get; private set; }

    public TokenPair Issue(AuthPrincipal principal)
    {
        IssuedFor = principal;
        return TokenToIssue;
    }

    public AuthPrincipal ValidateRefreshToken(string refreshToken) => RefreshResult;
}
