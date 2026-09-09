namespace Marketplace.Domain.Reports;

/// <summary>A flag raised against a listing by a visitor, resolved by an admin.</summary>
public class Report : Entity<Guid>, IAggregateRoot
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

    /// <summary>
    /// Resolves an open report — either after removing the listing (RESOLVED_REMOVED) or clearing it
    /// (RESOLVED_CLEARED). Records which admin resolved it and when (FR-021, FR-030).
    /// </summary>
    public void Resolve(Guid adminId, ReportStatus resolution, DateTime now)
    {
        if (Status != ReportStatus.OPEN)
        {
            throw new DomainRuleException("Only an open report can be resolved.");
        }

        if (resolution is not (ReportStatus.RESOLVED_REMOVED or ReportStatus.RESOLVED_CLEARED))
        {
            throw new DomainRuleException("A report must be resolved as removed or cleared.");
        }

        Status = resolution;
        ResolvedByAdminId = adminId;
        ResolvedAt = now;
    }
}
