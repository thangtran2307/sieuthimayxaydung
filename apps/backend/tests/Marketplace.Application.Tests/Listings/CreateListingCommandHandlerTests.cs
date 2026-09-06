using Marketplace.Application.Listings;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Listings;

public sealed class CreateListingCommandHandlerTests
{
    private readonly FakeCurrentUser _currentUser = new() { UserId = Guid.NewGuid() };
    private readonly FakeCategoryQueries _categories = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly StubClock _clock = new(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    private readonly Guid _categoryId;

    public CreateListingCommandHandlerTests()
    {
        _categoryId = _categories.AddCategory();
    }

    private CreateListingCommandHandler Handler => new(_currentUser, _categories, _unitOfWork, _clock);

    private CreateListingRequest Request(Guid? categoryId = null) => new(
        categoryId ?? _categoryId,
        null,
        "Máy đào Komatsu PC200",
        Condition.USED,
        850_000_000,
        false,
        "Hà Nội",
        "Well maintained.",
        new ListingSpecsInput(2018, "Komatsu", "PC200", 5000, "Japan", "20t"),
        [new ListingPhotoInput("https://cdn/1.jpg", 0, 800, 600)]);

    [Fact]
    public async Task Creates_a_pending_listing_owned_by_the_current_seller()
    {
        var result = await Handler.Handle(new CreateListingCommand(Request()), CancellationToken.None);

        var saved = Assert.Single(_unitOfWork.Listings.Added);
        Assert.Equal(_currentUser.UserId, saved.SellerId);
        Assert.Equal(ListingStatus.PENDING, saved.Status);
        Assert.Single(saved.Photos);
        Assert.StartsWith("may-dao-komatsu-pc200-", saved.Slug);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);

        Assert.Equal(saved.Id, result.Id);
        Assert.Equal(ListingStatus.PENDING, result.Status);
    }

    [Fact]
    public async Task Requires_an_authenticated_seller()
    {
        _currentUser.UserId = null;

        await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(new CreateListingCommand(Request()), CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Rejects_an_unknown_category()
    {
        var command = new CreateListingCommand(Request(categoryId: Guid.NewGuid()));

        var ex = await Assert.ThrowsAsync<DomainRuleException>(
            () => Handler.Handle(command, CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.CategoryNotFound, ex.Code);
        Assert.Empty(_unitOfWork.Listings.Added);
    }

    [Fact]
    public async Task Rejects_a_subcategory_from_a_different_category()
    {
        var otherParent = _categories.AddCategory();
        var mismatchedSub = _categories.AddSubcategory(otherParent);
        var request = Request() with { SubcategoryId = mismatchedSub };

        var ex = await Assert.ThrowsAsync<DomainRuleException>(
            () => Handler.Handle(new CreateListingCommand(request), CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.InvalidSubcategory, ex.Code);
    }
}
