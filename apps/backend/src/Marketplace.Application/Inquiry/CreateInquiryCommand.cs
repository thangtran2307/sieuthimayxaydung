using Marketplace.Application.Catalog;
using Marketplace.Application.Common.Persistence;
using Mediator;
using InquiryEntity = Marketplace.Domain.Inquiry.Inquiry;

namespace Marketplace.Application.Inquiry;

/// <summary>Records a buyer's contact against a listing (no account required).</summary>
public sealed record CreateInquiryCommand(Guid ListingId, CreateInquiryRequest Body)
    : ICommand<InquiryResultDto>;

public sealed class CreateInquiryCommandHandler(
    IListingQueries listingQueries,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<CreateInquiryCommand, InquiryResultDto>
{
    public async ValueTask<InquiryResultDto> Handle(
        CreateInquiryCommand command,
        CancellationToken cancellationToken)
    {
        var contact = await listingQueries.GetListingContactAsync(command.ListingId, cancellationToken);
        if (contact is null || contact.Status != ListingStatus.ACTIVE)
        {
            throw new NotFoundException("Listing not found", ErrorCodes.ListingNotFound);
        }

        var body = command.Body;
        var inquiry = body.Type == InquiryType.PHONE_REVEAL
            ? InquiryEntity.PhoneReveal(contact.ListingId, contact.SellerId, clock.UtcNow)
            : InquiryEntity.SendMessage(
                contact.ListingId,
                contact.SellerId,
                body.BuyerName,
                body.Message,
                clock.UtcNow,
                body.BuyerPhone,
                body.BuyerEmail);

        await unitOfWork.InquiryRepository.AddAsync(inquiry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new InquiryResultDto(
            body.Type,
            body.Type == InquiryType.PHONE_REVEAL ? contact.SellerPhone : null);
    }
}
