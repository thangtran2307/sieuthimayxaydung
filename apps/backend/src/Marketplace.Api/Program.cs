using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Marketplace.Api.Http;
using Marketplace.Api.Middleware;
using Marketplace.Api.Security;
using Marketplace.Api.Startup;
using Marketplace.Api.Swagger;
using Marketplace.Api.Validation;
using Marketplace.Application;
using Marketplace.Application.Common.Auth;
using Marketplace.Infrastructure;
using Marketplace.Infrastructure.Configuration;
using Marketplace.Infrastructure.Modules;
using Marketplace.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ── Logging: human-readable console in Development, structured JSON otherwise ────
builder.Host.UseSerilog(LoggingSetup.ConfigureSerilog);

// ── Autofac container ───────────────────────────────────────────────────────────
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// ── Options binding + validation (fail loud on misconfig) ───────────────────────
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(
        o => !string.IsNullOrWhiteSpace(o.AccessSecret) && !string.IsNullOrWhiteSpace(o.RefreshSecret),
        "Jwt:AccessSecret and Jwt:RefreshSecret must be configured.")
    .ValidateOnStart();
builder.Services.AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection(StorageOptions.SectionName));
builder.Services.AddOptions<InternalOptions>()
    .Bind(builder.Configuration.GetSection(InternalOptions.SectionName));

// ── Persistence (EF Core write side + Dapper read side) ─────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── Application layer: CQRS mediator (source-generated) + AutoMapper, then validators ───────────
builder.Services.AddApplication();
builder.Services.AddValidatorsFromAssembly(ApplicationAssembly.Reference);

// ── MVC + envelope/validation filters ───────────────────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new ApiPrefixConvention());
    options.Filters.Add<FluentValidationFilter>();
    options.Filters.Add<EnvelopeResultFilter>();
}).AddJsonOptions(options =>
{
    // Serialize enums as their UPPER_SNAKE names (matches the OpenAPI/Zod contract).
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// ── API versioning (url segment: /api/v1/...) ───────────────────────────────────
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ── Swagger / OpenAPI (one document per API version) ────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// ── Health checks (.NET) ────────────────────────────────────────────────────────
builder.Services.AddHealthChecks().AddDbContextCheck<MarketplaceDbContext>("database");

// ── Authentication (JWT from httpOnly cookie) + authorization ───────────────────
builder.Services.AddCookieJwtAuthentication(builder.Configuration);

// ── Current-user accessor: reads the authenticated caller from the request's JWT claims ──
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// ── CORS for the frontend origin (required; default lives in appsettings, not in code) ──
string frontendUrl = builder.Configuration["FrontendUrl"]
    ?? throw new InvalidOperationException("FrontendUrl must be configured.");
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(frontendUrl).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

// ── Autofac modules ─────────────────────────────────────────────────────────────
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
    container.RegisterModule(new InfrastructureModule()));

var app = builder.Build();

// ── CLI verbs: `migrate` and `seed` (run and exit) ──────────────────────────────
if (await MaintenanceCli.TryRunAsync(app, args))
{
    return;
}

// ── HTTP pipeline ───────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseVersionedSwaggerUi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/api/health");

await app.RunAsync();
