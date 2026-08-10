using System.Text.Json;

namespace Marketplace.Application.Catalog;

/// <summary>A single photo on a listing (gallery order via <see cref="SortOrder"/>; 0 = primary).</summary>
public sealed record ListingPhotoDto(string Url, int SortOrder, int? Width, int? Height);

/// <summary>Seller information safe for public exposure (never credentials).</summary>
public sealed record PublicSellerDto(
    Guid Id,
    string DisplayName,
    string LocationProvince,
    bool Verified,
    DateTime JoinedAt);

/// <summary>Full public listing detail (mirrors the OpenAPI <c>Listing</c>).</summary>
public sealed record ListingDetailDto(
    Guid Id,
    string Slug,
    string Title,
    Condition Condition,
    long? PriceAmount,
    bool PriceContact,
    string Currency,
    string LocationProvince,
    string Description,
    IReadOnlyDictionary<string, JsonElement> Specs,
    IReadOnlyList<ListingPhotoDto> Photos,
    ListingStatus Status,
    bool Boosted,
    CategoryDto Category,
    CategoryDto Subcategory,
    PublicSellerDto Seller,
    int ViewCount,
    DateTime CreatedAt,
    DateTime? PublishedAt);
