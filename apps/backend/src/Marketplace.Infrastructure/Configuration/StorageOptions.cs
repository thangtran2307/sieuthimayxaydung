namespace Marketplace.Infrastructure.Configuration;

/// <summary>S3-compatible object storage settings bound from the `Storage` config section.</summary>
public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string Endpoint { get; set; } = string.Empty;

    public string Region { get; set; } = "us-east-1";

    public string Bucket { get; set; } = "listings";

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;
}
