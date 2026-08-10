using Npgsql;

namespace Marketplace.Infrastructure.Persistence;

/// <summary>
/// Creates read-only PostgreSQL connections for the Dapper query side. Uses the dedicated read
/// connection string so reads can target a read replica independently of the write (EF) connection.
/// Each query opens and disposes its own connection.
/// </summary>
public sealed class ReadDbConnectionFactory(string connectionString)
{
    public NpgsqlConnection Create() => new(connectionString);
}
