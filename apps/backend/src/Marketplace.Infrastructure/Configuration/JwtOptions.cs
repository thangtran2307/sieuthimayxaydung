namespace Marketplace.Infrastructure.Configuration;

/// <summary>JWT signing configuration bound from the `Jwt` config section.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string AccessSecret { get; set; } = string.Empty;

    public string RefreshSecret { get; set; } = string.Empty;

    public int AccessTtlMinutes { get; set; } = 15;

    public int RefreshTtlDays { get; set; } = 30;

    public string Issuer { get; set; } = "marketplace";

    public string Audience { get; set; } = "marketplace";
}
