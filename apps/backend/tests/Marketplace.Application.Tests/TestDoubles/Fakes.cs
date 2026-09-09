using System.Linq.Expressions;
using Marketplace.Application.Categories;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Common.Persistence;
using Marketplace.Application.Identities;
using Marketplace.Application.Listings;
using Marketplace.Application.Moderation;
using Marketplace.Domain.Common;
using Marketplace.Domain.Identities;
using Marketplace.Domain.Inquiries;
using Marketplace.Domain.Listings;
using Marketplace.Domain.ModerationDecisions;
using Marketplace.Domain.Reports;

namespace Marketplace.Application.Tests.TestDoubles;

/// <summary>In-memory read side for the moderation views.</summary>
internal sealed class FakeModerationQueries : IModerationQueries
{
    public IReadOnlyList<ModerationQueueItemDto> PendingResult { get; set; } = [];

    public IReadOnlyList<ReportDto> ReportsResult { get; set; } = [];

    public Task<IReadOnlyList<ModerationQueueItemDto>> GetPendingListingsAsync(
        CancellationToken cancellationToken = default) => Task.FromResult(PendingResult);

    public Task<IReadOnlyList<ReportDto>> GetOpenReportsAsync(
        CancellationToken cancellationToken = default) => Task.FromResult(ReportsResult);
}

/// <summary>In-memory user read side.</summary>
internal sealed class FakeUserQueries : IUserQueries
{
    public AuthUserDto Result { get; set; }

    public Task<AuthUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result);
}

/// <summary>In-memory category read side; nodes are looked up by id from a seeded list.</summary>
internal sealed class FakeCategoryQueries : ICategoryQueries
{
    public List<CategoryDto> Categories { get; } = [];

    /// <summary>Seeds a top-level category and returns its id.</summary>
    public Guid AddCategory(Guid? id = null)
    {
        var categoryId = id ?? Guid.NewGuid();
        Categories.Add(new CategoryDto(categoryId, "excavators", null, "Máy đào", "Excavators", "truck", 0));
        return categoryId;
    }

    /// <summary>Seeds a subcategory under <paramref name="parentId"/> and returns its id.</summary>
    public Guid AddSubcategory(Guid parentId, Guid? id = null)
    {
        var subId = id ?? Guid.NewGuid();
        Categories.Add(new CategoryDto(subId, "crawler", parentId, "Bánh xích", "Crawler", null, 0));
        return subId;
    }

    public Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CategoryDto>>(Categories);

    public Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(Categories.FirstOrDefault(c => c.Id == id));
}

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

    public SellerListingDetailDto SellerListingResult { get; set; }

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

    public Task<SellerListingDetailDto> GetSellerListingAsync(
        Guid sellerId,
        Guid listingId,
        CancellationToken cancellationToken = default)
    {
        CapturedSellerId = sellerId;
        return Task.FromResult(SellerListingResult);
    }
}

/// <summary>Generic in-memory base for per-aggregate fake repositories.</summary>
internal class FakeRepository<T> : IRepository<T>
    where T : Entity, IAggregateRoot
{
    public List<T> Added { get; } = [];

    public T ToReturn { get; set; }

    /// <summary>Rows returned (filtered by the predicate) from <see cref="ListAsync"/>.</summary>
    public List<T> Items { get; } = [];

    public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        Task.FromResult(ToReturn);

    public Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<T>>(Items.Where(predicate.Compile()).ToList());

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

internal sealed class FakeModerationDecisionRepository
    : FakeRepository<ModerationDecision>, IModerationDecisionRepository;

/// <summary>In-memory unit of work exposing the fake repositories and counting commits.</summary>
internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public FakeUserRepository Users { get; } = new();

    public FakeListingRepository Listings { get; } = new();

    public FakeInquiryRepository Inquiries { get; } = new();

    public FakeReportRepository Reports { get; } = new();

    public FakeModerationDecisionRepository ModerationDecisions { get; } = new();

    public int SaveChangesCallCount { get; private set; }

    IUserRepository IUnitOfWork.UserRepository => Users;

    IListingRepository IUnitOfWork.ListingRepository => Listings;

    IInquiryRepository IUnitOfWork.InquiryRepository => Inquiries;

    IReportRepository IUnitOfWork.ReportRepository => Reports;

    IModerationDecisionRepository IUnitOfWork.ModerationDecisionRepository => ModerationDecisions;

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
