using System.Text.Json;
using Marketplace.Application.Categories;
using Marketplace.Application.Listings;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.TestDoubles;

/// <summary>Builders for read-side DTOs used across the handler tests.</summary>
internal static class Fixtures
{
    public static ListingDetailDto Detail(Guid? id = null, int viewCount = 0) => new(
        id ?? Guid.NewGuid(),
        "excavator-abc123",
        "Komatsu PC200 Excavator",
        Condition.USED,
        850_000_000,
        false,
        "VND",
        "Hà Nội",
        "A well maintained excavator in great condition.",
        new Dictionary<string, JsonElement>(),
        [],
        ListingStatus.ACTIVE,
        false,
        new CategoryDto(Guid.NewGuid(), "excavator", null, "Máy đào", "Excavators", "truck", 0),
        null,
        new PublicSellerDto(Guid.NewGuid(), "Seller Co.", "Hà Nội", true, DateTime.UtcNow),
        viewCount,
        DateTime.UtcNow,
        DateTime.UtcNow);
}
