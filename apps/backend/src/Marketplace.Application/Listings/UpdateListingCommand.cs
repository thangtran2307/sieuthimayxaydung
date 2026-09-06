using FluentValidation;
using Marketplace.Application.Categories;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Mediator;

namespace Marketplace.Application.Listings;

/// <summary>Editable fields of a listing (photo management is a separate concern).</summary>
public sealed record UpdateListingRequest(
    Guid CategoryId,
    Guid? SubcategoryId,
    string Title,
    Condition Condition,
    long? PriceAmount,
    bool PriceContact,
    string LocationProvince,
    string Description,
    ListingSpecsInput Specs);

public sealed class UpdateListingRequestValidator : AbstractValidator<UpdateListingRequest>
{
    public UpdateListingRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Condition).IsInEnum();
        RuleFor(x => x.LocationProvince).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);

        When(x => !x.PriceContact, () =>
            RuleFor(x => x.PriceAmount).NotNull().GreaterThan(0));
    }
}

/// <summary>Applies a seller's edits to their own listing (FR-017).</summary>
public sealed record UpdateListingCommand(Guid ListingId, UpdateListingRequest Body) : ICommand;

public sealed class UpdateListingCommandHandler(
    ICurrentUser currentUser,
    ICategoryQueries categoryQueries,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<UpdateListingCommand>
{
    public async ValueTask<Unit> Handle(UpdateListingCommand command, CancellationToken cancellationToken)
    {
        // Ownership first (403/404 take precedence over payload validation), then the category check.
        var listing = await SellerListing.LoadOwnedAsync(
            unitOfWork,
            currentUser,
            command.ListingId,
            cancellationToken);

        await ListingCategory.EnsureValidAsync(
            categoryQueries,
            command.Body.CategoryId,
            command.Body.SubcategoryId,
            cancellationToken);

        listing.UpdateDetails(command.Body.ToDetails(), clock.UtcNow);

        unitOfWork.ListingRepository.Update(listing);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
