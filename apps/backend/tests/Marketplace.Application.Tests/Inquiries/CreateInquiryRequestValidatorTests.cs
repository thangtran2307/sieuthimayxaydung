using Marketplace.Application.Inquiries;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Inquiries;

public sealed class CreateInquiryRequestValidatorTests
{
    private readonly CreateInquiryRequestValidator _validator = new();

    [Fact]
    public void Message_requires_name_and_message()
    {
        var result = _validator.Validate(new CreateInquiryRequest(InquiryType.MESSAGE, null, null, null, null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInquiryRequest.BuyerName));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInquiryRequest.Message));
    }

    [Fact]
    public void Message_with_name_and_message_is_valid()
    {
        var result = _validator.Validate(
            new CreateInquiryRequest(InquiryType.MESSAGE, "Buyer", null, null, "Still available?"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Phone_reveal_is_valid_without_contact_fields()
    {
        var result = _validator.Validate(new CreateInquiryRequest(InquiryType.PHONE_REVEAL, null, null, null, null));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Invalid_email_is_rejected()
    {
        var result = _validator.Validate(
            new CreateInquiryRequest(InquiryType.MESSAGE, "Buyer", null, "not-an-email", "Hello"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInquiryRequest.BuyerEmail));
    }
}
