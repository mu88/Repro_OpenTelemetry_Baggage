using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Web;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder, string serviceName)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services
            .AddLogging(
                logging => logging.AddOpenTelemetry(
                    openTelemetryLogging =>
                    {
                        openTelemetryLogging.IncludeFormattedMessage = true;
                        openTelemetryLogging.IncludeScopes = true;
                    }))
            .AddOpenTelemetry()
            .UseOtlpExporter()
            .ConfigureResource(c => c.AddService(serviceName))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(
                tracing => tracing
                    .SetSampler(new AlwaysOnSampler()) // https://github.com/open-telemetry/opentelemetry-dotnet/issues/4074
                    .AddSource(ActivityExtensions.ActivitySourceName) // https://github.com/open-telemetry/opentelemetry-dotnet/issues/1918#issuecomment-802294737
                    .AddConsoleExporter()
                    .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.EnrichWithIDbCommand = (activity, command) => activity.DisplayName = $"EF Core: {command.CommandType}";
                    }));

        return builder;
    }
}