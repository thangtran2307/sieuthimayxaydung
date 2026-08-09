using Dapper;

namespace Marketplace.Infrastructure.Persistence;

/// <summary>
/// One-time Npgsql/Dapper setup. Enables legacy timestamp behavior so C# <see cref="DateTime"/>
/// maps to <c>timestamp without time zone</c> (we always store UTC), and configures Dapper to map
/// snake_case columns to PascalCase properties for the read side.
/// </summary>
public static class NpgsqlBootstrap
{
    private static bool _configured;

    public static void Configure()
    {
        if (_configured)
        {
            return;
        }

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        _configured = true;
    }
}
