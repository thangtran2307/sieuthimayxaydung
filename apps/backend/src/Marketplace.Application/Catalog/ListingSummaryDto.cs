namespace Marketplace.Application.Catalog;

/// <summary>Card shape for search/listing rows (mirrors the OpenAPI <c>ListingSummary</c>).</summary>
public sealed record ListingSummaryDto(
    Guid Id,
    string Slug,
    string Title,
    long? PriceAmount,
    bool PriceContact,
    string Currency,
    Condition Condition,
    string LocationProvince,
    string ThumbnailUrl,
    bool Boosted,
    string CategorySlug,
    DateTime CreatedAt);
