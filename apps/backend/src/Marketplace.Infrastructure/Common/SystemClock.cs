namespace Marketplace.Infrastructure.Common;

/// <inheritdoc />
public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
