using Dapper;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Infrastructure.Persistence;

namespace Marketplace.Infrastructure.Seed;

/// <summary>
/// Seeds reference data from the embedded <c>seed.sql</c> script (categories, subcategories, boost
/// packages) and the admin account in C# (password hashing). Idempotent. Runs after migrations.
/// </summary>
public sealed class DataSeeder(MarketplaceDbContext dbContext, IPasswordHasher passwordHasher)
    : IDataSeeder
{
    private const string SeedScriptResource = "Marketplace.Infrastructure.Seed.seed.sql";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var connection = dbContext.Database.GetDbConnection();

        string script = ReadEmbeddedScript(SeedScriptResource);
        await connection.ExecuteAsync(new CommandDefinition(script, cancellationToken: cancellationToken));

        string adminHash = passwordHasher.Hash("ChangeMe123!");
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO users (email, password_hash, role, display_name, verified)
            VALUES (@email, @hash, 'ADMIN', @name, true)
            ON CONFLICT (email) DO NOTHING;
            """,
            new { email = "admin@smxd.local", hash = adminHash, name = "Marketplace Admin" },
            cancellationToken: cancellationToken));
    }

    private static string ReadEmbeddedScript(string resourceName)
    {
        var assembly = typeof(DataSeeder).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded seed resource '{resourceName}' not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
