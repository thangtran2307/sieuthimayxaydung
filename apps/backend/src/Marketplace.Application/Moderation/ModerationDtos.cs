namespace Marketplace.Application.Moderation;

/// <summary>A pending listing awaiting review, with the info an admin needs to decide (FR-019).</summary>
public sealed record ModerationQueueItemDto(
    Guid Id,
    string Slug,
    string Title,
    string SellerName,
    string CategorySlug,
    long? PriceAmount,
    bool PriceContact,
    string Currency,
    string LocationProvince,
    DateTime CreatedAt);

/// <summary>A report against a listing, shown in the admin report queue (FR-021).</summary>
public sealed record ReportDto(
    Guid Id,
    Guid ListingId,
    string ListingSlug,
    string ListingTitle,
    ListingStatus ListingStatus,
    ReportReason Reason,
    string Details,
    string ReporterContact,
    ReportStatus Status,
    DateTime CreatedAt);

/// <summary>How an admin resolves a report: remove the listing, or clear the report (FR-021).</summary>
public enum ReportResolution
{
    REMOVE,
    CLEAR,
}

/// <summary>Outcome of a bulk moderation action (FR-022).</summary>
public sealed record BulkModerationResultDto(int Succeeded, int Skipped);
