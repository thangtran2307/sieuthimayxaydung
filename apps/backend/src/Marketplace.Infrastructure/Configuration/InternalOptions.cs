namespace Marketplace.Infrastructure.Configuration;

/// <summary>
/// Settings for internal/maintenance endpoints (bound from the `Internal` config section). The API
/// key is a secret — supply it via environment variables or user-secrets, never commit it. When it
/// is empty, the internal endpoints are effectively disabled (all requests are rejected).
/// </summary>
public sealed class InternalOptions
{
    public const string SectionName = "Internal";

    public string ApiKey { get; set; } = string.Empty;
}
