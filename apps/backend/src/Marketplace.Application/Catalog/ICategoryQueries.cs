namespace Marketplace.Application.Catalog;

/// <summary>Read side (Dapper) for the category taxonomy. Runs on the read connection.</summary>
public interface ICategoryQueries
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
