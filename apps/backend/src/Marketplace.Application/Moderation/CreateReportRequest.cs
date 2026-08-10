using FluentValidation;

namespace Marketplace.Application.Moderation;

/// <summary>A visitor's flag against a listing (no account required).</summary>
public sealed record CreateReportRequest(ReportReason Reason, string Details, string ReporterContact);

public sealed class CreateReportRequestValidator : AbstractValidator<CreateReportRequest>
{
    public CreateReportRequestValidator()
    {
        RuleFor(x => x.Reason).IsInEnum();
        RuleFor(x => x.Details).MaximumLength(2000);
        RuleFor(x => x.ReporterContact).MaximumLength(255);
    }
}
