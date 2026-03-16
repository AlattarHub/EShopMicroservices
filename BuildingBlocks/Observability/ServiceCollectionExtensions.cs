using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Enrichers.Span;

namespace BuildingBlocks.Observability
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMicroserviceObservability(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddLogging();
            Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithSpan()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();

            services.AddHealthChecks();

            var serviceName = configuration["ServiceName"] ?? "Microservice";

            services.AddOpenTelemetry()
                .WithTracing(tracerProviderBuilder =>
                {
                    tracerProviderBuilder
                        .SetResourceBuilder(
                            ResourceBuilder.CreateDefault()
                                .AddService(serviceName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter(options =>
                        {
                            options.Endpoint = new Uri("http://jaeger:4317");
                        });
                });

            return services;
        }

    }
}
