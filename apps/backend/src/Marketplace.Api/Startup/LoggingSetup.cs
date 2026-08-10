using Serilog;
using Serilog.Formatting.Compact;

namespace Marketplace.Api.Startup;

/// <summary>Serilog configuration: human-readable console in Development, structured JSON otherwise.</summary>
internal static class LoggingSetup
{
    public static void ConfigureSerilog(HostBuilderContext context, LoggerConfiguration configuration)
    {
        configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
        if (context.HostingEnvironment.IsDevelopment())
        {
            // Human-readable, with a full local timestamp for easy scanning in the console.
            configuration.WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }
        else
        {
            // Structured JSON (includes an ISO-8601 @t timestamp) for log aggregation.
            configuration.WriteTo.Console(new RenderedCompactJsonFormatter());
        }
    }
}
