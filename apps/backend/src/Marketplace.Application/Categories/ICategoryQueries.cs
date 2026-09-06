namespace Marketplace.Application.Categories;

/// <summary>Read side (Dapper) for the category taxonomy. Runs on the read connection.</summary>
public interface ICategoryQueries
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a single taxonomy node by id, or null when it does not exist.</summary>
    Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
