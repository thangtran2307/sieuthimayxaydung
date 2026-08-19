using Marketplace.Domain.Reports;

namespace Marketplace.Infrastructure.Persistence.Repositories;

internal sealed class ReportRepository(MarketplaceDbContext dbContext)
    : BaseEfRepository<Report>(dbContext), IReportRepository;
