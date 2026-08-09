namespace Marketplace.Domain.Identity;

/// <summary>A registered identity that can own listings (SELLER) or moderate (ADMIN).</summary>
public class User : Entity
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
}
