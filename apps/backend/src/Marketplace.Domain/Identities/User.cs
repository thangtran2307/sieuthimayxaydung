namespace Marketplace.Domain.Identities;

/// <summary>A registered identity that can own listings (SELLER) or moderate (ADMIN).</summary>
public class User : Entity<Guid>, IAggregateRoot
{
    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public string GoogleId { get; private set; }

    public UserRole Role { get; private set; } = UserRole.SELLER;

    public string DisplayName { get; private set; }

    public string Phone { get; private set; }

    public string LocationProvince { get; private set; }

    public string Description { get; private set; }

    public string CoverImageUrl { get; private set; }

    public bool Verified { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Registers a new seller account (email/password). Any registered user may post immediately;
    /// trust is enforced by per-listing moderation, not account pre-approval (FR-013a).
    /// </summary>
    public static User Register(
        string email,
        string passwordHash,
        string displayName,
        DateTime now,
        string phone = null,
        string locationProvince = null) => new()
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            DisplayName = displayName,
            Phone = phone,
            LocationProvince = locationProvince,
            Role = UserRole.SELLER,
            Verified = false,
            CreatedAt = now,
            UpdatedAt = now,
        };
}
