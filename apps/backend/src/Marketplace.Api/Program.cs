using System.Security.Claims;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Marketplace.Api.Http;
using Marketplace.Api.Middleware;
using Marketplace.Api.Validation;
using Marketplace.Application;
using Marketplace.Application.Common.Persistence;
using Marketplace.Infrastructure;
using Marketplace.Infrastructure.Configuration;
using Marketplace.Infrastructure.Modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// ── Structured JSON logging (Constitution: structured logging) ──────────────────
builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter()));

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

// ── Persistence (EF Core write side + Dapper read side) ─────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── Application layer: CQRS mediator (source-generated) + AutoMapper, then validators ───────────
builder.Services.AddApplication();
builder.Services.AddValidatorsFromAssembly(ApplicationAssembly.Reference);

// ── MVC + envelope/validation filters + OpenAPI ─────────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new ApiPrefixConvention());
    options.Filters.Add<FluentValidationFilter>();
    options.Filters.Add<EnvelopeResultFilter>();
});
builder.Services.AddOpenApi();

// ── Authentication (JWT from httpOnly cookie) + authorization ───────────────────
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
var accessSecret = string.IsNullOrWhiteSpace(jwt.AccessSecret) ? new string('0', 32) : jwt.AccessSecret;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(accessSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = "sub",
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("access_token", out var token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddAuthorization(options =>
    options.AddPolicy("Admin", policy => policy.RequireRole(nameof(UserRole.ADMIN))));

// ── CORS for the frontend origin ────────────────────────────────────────────────
var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:3000";
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(frontendUrl).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

// ── Autofac modules ─────────────────────────────────────────────────────────────
builder.Host.ConfigureContainer<ContainerBuilder>(container =>
    container.RegisterModule(new InfrastructureModule()));

var app = builder.Build();

// ── CLI verbs: `migrate` and `seed` (run and exit) ──────────────────────────────
if (args.Length > 0 && args[0] is "migrate" or "seed")
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>().Migrate();
    if (args[0] == "seed")
    {
        await scope.ServiceProvider.GetRequiredService<IDataSeeder>().SeedAsync();
    }

    return;
}

// ── HTTP pipeline ───────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapOpenApi();

await app.RunAsync();
