using Marketplace.Domain.Listings;

namespace Marketplace.Infrastructure.Persistence.Repositories;

internal sealed class ListingRepository(MarketplaceDbContext dbContext)
    : BaseEfRepository<Listing>(dbContext), IListingRepository;
