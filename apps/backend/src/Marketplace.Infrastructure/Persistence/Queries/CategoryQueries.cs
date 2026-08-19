using Dapper;
using Marketplace.Application.Categories;

namespace Marketplace.Infrastructure.Persistence.Queries;

/// <summary>Dapper read side for the category taxonomy (runs on the read connection).</summary>
internal sealed class CategoryQueries(ReadDbConnectionFactory connectionFactory) : ICategoryQueries
{
    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, slug, parent_id, label_vi, label_en, icon, sort_order
            FROM categories
            ORDER BY parent_id NULLS FIRST, sort_order
            """;

        using var connection = connectionFactory.Create();
        var rows = await connection.QueryAsync<CategoryDto>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }
}
