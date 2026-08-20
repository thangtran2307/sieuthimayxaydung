using Dapper;
using Marketplace.Application.Samples;

namespace Marketplace.Infrastructure.Persistence.Queries;

public class SampleQueries(ReadDbConnectionFactory connectionFactory) : ISampleQueries
{
    public async Task<IReadOnlyList<SampleDto>> GetAllSamplesAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, field_one, field_two
            FROM Samples
        ";

        using var connection = connectionFactory.Create();
        var rows = await connection.QueryAsync<SampleDto>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<SampleDto> GetSampleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, field_one, field_two
            FROM Samples
            WHERE id = @Id
        ";

        using var connection = connectionFactory.Create();
        var row = await connection.QuerySingleOrDefaultAsync<SampleDto>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return row;
    }
}
