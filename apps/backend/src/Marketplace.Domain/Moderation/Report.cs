namespace Marketplace.Domain.Moderation;

/// <summary>A flag raised against a listing by a visitor, resolved by an admin.</summary>
public class Report : Entity
{
    public Guid ListingId { get; private set; }

    public ReportReason Reason { get; private set; }

    public string Details { get; private set; }

    public string ReporterContact { get; private set; }

    public ReportStatus Status { get; private set; } = ReportStatus.OPEN;

    public Guid? ResolvedByAdminId { get; private set; }

    public DateTime? ResolvedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
