using FluentValidation;
using Marketplace.Application.Categories;
using Marketplace.Application.Common;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.Listings;
using Mediator;

namespace Marketplace.Application.Listings;

/// <summary>Guided listing-creation payload (FR-014); at least one photo is required.</summary>
public sealed record CreateListingRequest(
    Guid CategoryId,
    Guid? SubcategoryId,
    string Title,
    Condition Condition,
    long? PriceAmount,
    bool PriceContact,
    string LocationProvince,
    string Description,
    ListingSpecsInput Specs,
    IReadOnlyList<ListingPhotoInput> Photos);

public sealed class CreateListingRequestValidator : AbstractValidator<CreateListingRequest>
{
    public CreateListingRequestValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Condition).IsInEnum();
        RuleFor(x => x.LocationProvince).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);

        // Price is required unless the seller chose "contact for price".
        When(x => !x.PriceContact, () =>
            RuleFor(x => x.PriceAmount).NotNull().GreaterThan(0));

        RuleFor(x => x.Photos).NotEmpty().WithMessage("At least one photo is required.");
        RuleForEach(x => x.Photos).ChildRules(photo =>
            photo.RuleFor(p => p.Url).NotEmpty().MaximumLength(2048));
    }
}

/// <summary>Creates a pending listing owned by the current seller (FR-014, FR-016).</summary>
public sealed record CreateListingCommand(CreateListingRequest Body) : ICommand<CreatedListingDto>;

public sealed class CreateListingCommandHandler(
    ICurrentUser currentUser,
    ICategoryQueries categoryQueries,
    IUnitOfWork unitOfWork,
    IClock clock) : ICommandHandler<CreateListingCommand, CreatedListingDto>
{
    public async ValueTask<CreatedListingDto> Handle(
        CreateListingCommand command,
        CancellationToken cancellationToken)
    {
        var sellerId = currentUser.RequireUserId();
        var body = command.Body;

        await ListingCategory.EnsureValidAsync(
            categoryQueries,
            body.CategoryId,
            body.SubcategoryId,
            cancellationToken);

        var listing = Listing.Create(sellerId, Slug.Generate(body.Title), body.ToDetails(), clock.UtcNow);

        foreach (var photo in body.Photos)
        {
            listing.AddPhoto(photo.Url, photo.SortOrder, photo.Width, photo.Height);
        }

        await unitOfWork.ListingRepository.AddAsync(listing, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatedListingDto(listing.Id, listing.Slug, listing.Status);
    }
}
