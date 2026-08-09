namespace Marketplace.Domain.Catalog;

/// <summary>
/// Value object holding a listing's technical specifications, persisted as a JSONB column
/// (mapped via EF Core OwnsOne + ToJson). EF binds JSON members to the constructor parameters.
/// </summary>
public sealed class ListingSpecs(
    int? year = null,
    string brand = null,
    string model = null,
    int? hours = null,
    string origin = null,
    string capacity = null)
{
    public int? Year { get; private set; } = year;

    public string Brand { get; private set; } = brand;

    public string Model { get; private set; } = model;

    public int? Hours { get; private set; } = hours;

    public string Origin { get; private set; } = origin;

    public string Capacity { get; private set; } = capacity;
}
