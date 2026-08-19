using Marketplace.Domain.Common;
using Marketplace.Domain.Reports;

namespace Marketplace.Domain.Tests.Reports;

public sealed class ReportTests
{
    [Fact]
    public void Create_opens_a_report_with_the_given_reason()
    {
        var listingId = Guid.NewGuid();
        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var report = Report.Create(listingId, ReportReason.FRAUD, now, "Fake listing", "reporter@example.com");

        Assert.NotEqual(Guid.Empty, report.Id);
        Assert.Equal(listingId, report.ListingId);
        Assert.Equal(ReportReason.FRAUD, report.Reason);
        Assert.Equal(ReportStatus.OPEN, report.Status);
        Assert.Equal("Fake listing", report.Details);
        Assert.Equal("reporter@example.com", report.ReporterContact);
        Assert.Equal(now, report.CreatedAt);
        Assert.Null(report.ResolvedByAdminId);
        Assert.Null(report.ResolvedAt);
    }
}
