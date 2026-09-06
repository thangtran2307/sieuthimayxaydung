using Dapper;
using Marketplace.Application.Identities;

namespace Marketplace.Infrastructure.Persistence.Queries;

/// <summary>Dapper read side for user accounts (runs on the read connection).</summary>
internal sealed class UserQueries(ReadDbConnectionFactory connectionFactory) : IUserQueries
{
    public async Task<AuthUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Column order matches the AuthUserDto constructor (Dapper binds positional records by order).
        const string sql = """
            SELECT id, email, display_name, role
            FROM users
            WHERE id = @id
            """;

        using var connection = connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<AuthUserDto>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
    }
}
