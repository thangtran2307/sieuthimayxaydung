using Marketplace.Domain.Inquiries;

namespace Marketplace.Infrastructure.Persistence.Repositories;

internal sealed class InquiryRepository(MarketplaceDbContext dbContext)
    : BaseEfRepository<Inquiry>(dbContext), IInquiryRepository;
