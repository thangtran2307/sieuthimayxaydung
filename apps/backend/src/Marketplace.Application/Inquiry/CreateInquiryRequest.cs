using FluentValidation;

namespace Marketplace.Application.Inquiry;

/// <summary>
/// Buyer contact payload. For <see cref="InquiryType.MESSAGE"/>, name and message are required;
/// for <see cref="InquiryType.PHONE_REVEAL"/> all contact fields are optional.
/// </summary>
public sealed record CreateInquiryRequest(
    InquiryType Type,
    string BuyerName,
    string BuyerPhone,
    string BuyerEmail,
    string Message);

/// <summary>Contact result — the seller phone is returned only for a phone reveal.</summary>
public sealed record InquiryResultDto(InquiryType Type, string SellerPhone);

public sealed class CreateInquiryRequestValidator : AbstractValidator<CreateInquiryRequest>
{
    public CreateInquiryRequestValidator()
    {
        RuleFor(x => x.Type).IsInEnum();

        When(x => x.Type == InquiryType.MESSAGE, () =>
        {
            RuleFor(x => x.BuyerName).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
        });

        RuleFor(x => x.BuyerEmail)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.BuyerEmail));

        RuleFor(x => x.BuyerPhone).MaximumLength(32);
    }
}
