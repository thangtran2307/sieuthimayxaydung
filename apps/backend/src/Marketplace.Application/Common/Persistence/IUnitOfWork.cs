namespace Marketplace.Application.Common.Persistence;

/// <summary>
/// Commits all changes tracked across repositories in one transaction (Unit of Work pattern).
/// Command handlers call this once at the end of a use-case.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
