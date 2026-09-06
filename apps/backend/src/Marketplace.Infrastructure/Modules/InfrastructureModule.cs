using Autofac;
using Marketplace.Application.Categories;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Application.Identities;
using Marketplace.Application.Listings;
using Marketplace.Infrastructure.Auth;
using Marketplace.Infrastructure.Common;
using Marketplace.Infrastructure.Migrations;
using Marketplace.Infrastructure.Persistence;
using Marketplace.Infrastructure.Persistence.Queries;
using Marketplace.Infrastructure.Seed;

namespace Marketplace.Infrastructure.Modules;

/// <summary>
/// Autofac registrations for infrastructure services. The DbContext is registered via
/// <c>AddInfrastructure</c> (IServiceCollection) and populated into this container.
/// </summary>
public sealed class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Write side: per-aggregate repositories are reached through IUnitOfWork's named properties
        // (single write entry point), so they are not registered for direct injection.
        builder.RegisterType<EfUnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

        // Read side: Dapper query classes on the read connection (CQRS split).
        builder.RegisterType<CategoryQueries>().As<ICategoryQueries>().InstancePerLifetimeScope();
        builder.RegisterType<ListingQueries>().As<IListingQueries>().InstancePerLifetimeScope();
        builder.RegisterType<UserQueries>().As<IUserQueries>().InstancePerLifetimeScope();
        builder.RegisterType<SystemClock>().As<IClock>().SingleInstance();
        builder.RegisterType<PasswordHasherAdapter>().As<IPasswordHasher>().SingleInstance();
        builder.RegisterType<JwtTokenService>().As<IJwtTokenService>().SingleInstance();
        builder.RegisterType<EfCoreMigrator>().As<IDatabaseMigrator>().InstancePerLifetimeScope();
        builder.RegisterType<DataSeeder>().As<IDataSeeder>().InstancePerLifetimeScope();
    }
}
