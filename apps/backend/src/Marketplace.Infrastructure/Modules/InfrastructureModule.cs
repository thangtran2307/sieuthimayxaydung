using Autofac;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Infrastructure.Auth;
using Marketplace.Infrastructure.Common;
using Marketplace.Infrastructure.Migrations;
using Marketplace.Infrastructure.Persistence;
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
        builder.RegisterGeneric(typeof(EfRepository<>))
            .As(typeof(IRepository<>))
            .InstancePerLifetimeScope();

        builder.RegisterType<EfUnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
        builder.RegisterType<SystemClock>().As<IClock>().SingleInstance();
        builder.RegisterType<PasswordHasherAdapter>().As<IPasswordHasher>().SingleInstance();
        builder.RegisterType<JwtTokenService>().As<IJwtTokenService>().SingleInstance();
        builder.RegisterType<EfCoreMigrator>().As<IDatabaseMigrator>().InstancePerLifetimeScope();
        builder.RegisterType<DataSeeder>().As<IDataSeeder>().InstancePerLifetimeScope();
    }
}
