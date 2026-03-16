using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Observability
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMicroserviceObservability(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddLogging();

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
