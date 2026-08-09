namespace Marketplace.Application.Common.Persistence;

/// <summary>Applies pending schema migrations (implemented with DbUp in Infrastructure).</summary>
public interface IDatabaseMigrator
{
    void Migrate();
}
