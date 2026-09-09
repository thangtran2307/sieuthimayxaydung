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

    [Fact]
    public void Resolve_records_the_admin_resolution()
    {
        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var report = Report.Create(Guid.NewGuid(), ReportReason.SPAM, now);
        var adminId = Guid.NewGuid();

        report.Resolve(adminId, ReportStatus.RESOLVED_REMOVED, now.AddHours(1));

        Assert.Equal(ReportStatus.RESOLVED_REMOVED, report.Status);
        Assert.Equal(adminId, report.ResolvedByAdminId);
        Assert.Equal(now.AddHours(1), report.ResolvedAt);
    }

    [Fact]
    public void Resolve_rejects_a_non_open_report()
    {
        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var report = Report.Create(Guid.NewGuid(), ReportReason.SPAM, now);
        report.Resolve(Guid.NewGuid(), ReportStatus.RESOLVED_CLEARED, now);

        Assert.Throws<DomainRuleException>(
            () => report.Resolve(Guid.NewGuid(), ReportStatus.RESOLVED_REMOVED, now));
    }

    [Fact]
    public void Resolve_rejects_a_non_resolution_status()
    {
        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var report = Report.Create(Guid.NewGuid(), ReportReason.SPAM, now);

        Assert.Throws<DomainRuleException>(() => report.Resolve(Guid.NewGuid(), ReportStatus.OPEN, now));
    }
}
