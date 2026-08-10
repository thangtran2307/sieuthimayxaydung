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

    /// <summary>Raises a new flag against a listing (anyone may report; admins resolve later).</summary>
    public static Report Create(
        Guid listingId,
        ReportReason reason,
        DateTime now,
        string details = null,
        string reporterContact = null) => new()
        {
            Id = Guid.NewGuid(),
            ListingId = listingId,
            Reason = reason,
            Details = details,
            ReporterContact = reporterContact,
            Status = ReportStatus.OPEN,
            CreatedAt = now,
        };
}
