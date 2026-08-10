using Marketplace.Application.Inquiry;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Inquiry;

public sealed class CreateInquiryCommandHandlerTests
{
    private readonly FakeListingQueries _queries = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly StubClock _clock = new(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    private CreateInquiryCommandHandler Handler => new(_queries, _unitOfWork, _clock);

    [Theory]
    [InlineData(null)]
    [InlineData(ListingStatus.PENDING)]
    [InlineData(ListingStatus.SOLD)]
    public async Task Throws_not_found_when_listing_is_not_active(ListingStatus? status)
    {
        _queries.ContactResult = status is null
            ? null
            : new ListingContactStub(status.Value).Value;

        var command = new CreateInquiryCommand(
            Guid.NewGuid(),
            new CreateInquiryRequest(InquiryType.PHONE_REVEAL, null, null, null, null));

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => Handler.Handle(command, CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.ListingNotFound, ex.Code);
    }

    [Fact]
    public async Task Phone_reveal_returns_seller_phone_and_records_the_inquiry()
    {
        var listingId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        _queries.ContactResult = new Marketplace.Application.Catalog.ListingContact(
            listingId, sellerId, ListingStatus.ACTIVE, "0900000000");

        var command = new CreateInquiryCommand(
            listingId,
            new CreateInquiryRequest(InquiryType.PHONE_REVEAL, null, null, null, null));

        var result = await Handler.Handle(command, CancellationToken.None);

        Assert.Equal(InquiryType.PHONE_REVEAL, result.Type);
        Assert.Equal("0900000000", result.SellerPhone);
        var recorded = Assert.Single(_unitOfWork.Inquiries.Added);
        Assert.Equal(InquiryType.PHONE_REVEAL, recorded.Type);
        Assert.Equal(sellerId, recorded.SellerId);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Message_does_not_expose_seller_phone()
    {
        var listingId = Guid.NewGuid();
        _queries.ContactResult = new Marketplace.Application.Catalog.ListingContact(
            listingId, Guid.NewGuid(), ListingStatus.ACTIVE, "0900000000");

        var command = new CreateInquiryCommand(
            listingId,
            new CreateInquiryRequest(InquiryType.MESSAGE, "Buyer", null, null, "Is this available?"));

        var result = await Handler.Handle(command, CancellationToken.None);

        Assert.Null(result.SellerPhone);
        var recorded = Assert.Single(_unitOfWork.Inquiries.Added);
        Assert.Equal(InquiryType.MESSAGE, recorded.Type);
        Assert.Equal("Buyer", recorded.BuyerName);
        Assert.Equal("Is this available?", recorded.Message);
    }

    private readonly struct ListingContactStub(ListingStatus status)
    {
        public Marketplace.Application.Catalog.ListingContact Value { get; } =
            new(Guid.NewGuid(), Guid.NewGuid(), status, "0900000000");
    }
}
