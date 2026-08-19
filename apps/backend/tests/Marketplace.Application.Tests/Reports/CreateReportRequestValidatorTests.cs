using Marketplace.Application.Reports;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Reports;

public sealed class CreateReportRequestValidatorTests
{
    private readonly CreateReportRequestValidator _validator = new();

    [Fact]
    public void A_reason_only_report_is_valid()
    {
        var result = _validator.Validate(new CreateReportRequest(ReportReason.OTHER, null, null));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Overlong_details_are_rejected()
    {
        var result = _validator.Validate(
            new CreateReportRequest(ReportReason.OTHER, new string('x', 2001), null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateReportRequest.Details));
    }
}
