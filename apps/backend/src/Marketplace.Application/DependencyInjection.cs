using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Application;

/// <summary>
/// Registers Application-layer services. AddMediator MUST be called here — this project references
/// Mediator.SourceGenerator, so the generator sees the call and bakes the matching (Scoped)
/// lifetime. Calling it from another assembly causes a lifetime mismatch at startup.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
        return services;
    }
}
