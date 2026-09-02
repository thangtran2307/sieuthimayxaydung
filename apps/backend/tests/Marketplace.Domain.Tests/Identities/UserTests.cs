using Marketplace.Domain.Common;
using Marketplace.Domain.Identities;

namespace Marketplace.Domain.Tests.Identities;

public sealed class UserTests
{
    private static readonly DateTime Now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Register_creates_an_unverified_seller()
    {
        var user = User.Register("seller@example.com", "hash", "Seller Co.", Now, "0900000000", "Hà Nội");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("seller@example.com", user.Email);
        Assert.Equal("hash", user.PasswordHash);
        Assert.Equal("Seller Co.", user.DisplayName);
        Assert.Equal("0900000000", user.Phone);
        Assert.Equal("Hà Nội", user.LocationProvince);
        Assert.Equal(UserRole.SELLER, user.Role);
        Assert.False(user.Verified);
        Assert.Equal(Now, user.CreatedAt);
        Assert.Equal(Now, user.UpdatedAt);
    }

    [Fact]
    public void Register_allows_optional_contact_details_to_be_null()
    {
        var user = User.Register("seller@example.com", "hash", "Seller Co.", Now);

        Assert.Null(user.Phone);
        Assert.Null(user.LocationProvince);
    }
}
