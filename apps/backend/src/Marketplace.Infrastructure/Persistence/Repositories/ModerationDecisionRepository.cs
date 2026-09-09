using Marketplace.Domain.ModerationDecisions;

namespace Marketplace.Infrastructure.Persistence.Repositories;

internal sealed class ModerationDecisionRepository(MarketplaceDbContext dbContext)
    : BaseEfRepository<ModerationDecision>(dbContext), IModerationDecisionRepository;
