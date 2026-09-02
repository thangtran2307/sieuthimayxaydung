using Marketplace.Domain.Identities;

namespace Marketplace.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(MarketplaceDbContext dbContext)
    : BaseEfRepository<User>(dbContext), IUserRepository;
