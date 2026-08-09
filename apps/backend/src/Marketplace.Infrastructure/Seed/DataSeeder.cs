using Dapper;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Infrastructure.Persistence;

namespace Marketplace.Infrastructure.Seed;

/// <summary>
/// Seeds reference + demo data mirroring the design prototype: category taxonomy, boost packages,
/// and an admin account. Idempotent via ON CONFLICT. Uses Dapper over the DbContext's connection.
/// </summary>
public sealed class DataSeeder(MarketplaceDbContext dbContext, IPasswordHasher passwordHasher)
    : IDataSeeder
{
    private static readonly (string Slug, string Vi, string En, string Icon)[] Categories =
    [
        ("excavator", "Máy đào", "Excavators", "truck"),
        ("forklift", "Xe nâng", "Forklifts", "forklift"),
        ("crane", "Cần cẩu", "Cranes", "construction"),
        ("concrete", "Máy trộn & bơm", "Concrete Equipment", "cog"),
        ("road", "Máy lu & làm đường", "Road Equipment", "mountain"),
        ("parts", "Phụ tùng & linh kiện", "Parts & Components", "wrench"),
    ];

    private static readonly (string Tier, string Name, int Priority, int Days, long Price)[] Packages =
    [
        ("BASIC", "Basic", 1, 7, 200_000),
        ("FEATURED", "Featured", 5, 14, 500_000),
        ("MAX", "Max Priority", 10, 30, 1_200_000),
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var connection = dbContext.Database.GetDbConnection();

        var order = 0;
        foreach (var (slug, vi, en, icon) in Categories)
        {
            await connection.ExecuteAsync(new CommandDefinition(
                """
                INSERT INTO categories (slug, label_vi, label_en, icon, sort_order)
                VALUES (@slug, @vi, @en, @icon, @order)
                ON CONFLICT (slug) DO UPDATE
                    SET label_vi = EXCLUDED.label_vi,
                        label_en = EXCLUDED.label_en,
                        icon = EXCLUDED.icon,
                        sort_order = EXCLUDED.sort_order;
                """,
                new { slug, vi, en, icon, order },
                cancellationToken: cancellationToken));
            order++;
        }

        foreach (var (tier, name, priority, days, price) in Packages)
        {
            await connection.ExecuteAsync(new CommandDefinition(
                """
                INSERT INTO boost_packages (tier, name, priority_level, duration_days, price_amount)
                SELECT @tier, @name, @priority, @days, @price
                WHERE NOT EXISTS (SELECT 1 FROM boost_packages WHERE tier = @tier);
                """,
                new { tier, name, priority, days, price },
                cancellationToken: cancellationToken));
        }

        var adminHash = passwordHasher.Hash("ChangeMe123!");
        await connection.ExecuteAsync(new CommandDefinition(
            """
            INSERT INTO users (email, password_hash, role, display_name, verified)
            VALUES (@email, @hash, 'ADMIN', @name, true)
            ON CONFLICT (email) DO NOTHING;
            """,
            new { email = "admin@smxd.local", hash = adminHash, name = "Marketplace Admin" },
            cancellationToken: cancellationToken));
    }
}
