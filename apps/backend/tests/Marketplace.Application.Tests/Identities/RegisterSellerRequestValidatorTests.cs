using Marketplace.Application.Identities;

namespace Marketplace.Application.Tests.Identities;

public sealed class RegisterSellerRequestValidatorTests
{
    private readonly RegisterSellerRequestValidator _validator = new();

    private static RegisterSellerRequest Valid() =>
        new("seller@example.com", "supersecret", "Seller Co.", "0900000000", "Hà Nội");

    [Fact]
    public void Accepts_a_complete_request()
    {
        Assert.True(_validator.Validate(Valid()).IsValid);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    public void Rejects_an_invalid_email(string email)
    {
        Assert.False(_validator.Validate(Valid() with { Email = email }).IsValid);
    }

    [Fact]
    public void Rejects_a_short_password()
    {
        Assert.False(_validator.Validate(Valid() with { Password = "short" }).IsValid);
    }

    [Fact]
    public void Requires_a_display_name()
    {
        Assert.False(_validator.Validate(Valid() with { DisplayName = "" }).IsValid);
    }
}
