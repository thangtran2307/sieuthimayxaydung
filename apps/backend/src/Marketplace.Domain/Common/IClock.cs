namespace Marketplace.Domain.Common;

/// <summary>Abstraction over the system clock for testable time-dependent logic. Always UTC.</summary>
public interface IClock
{
    DateTime UtcNow { get; }
}
