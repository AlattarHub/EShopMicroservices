using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Observability
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomObservability(
        this IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddLogging();

            return services;
        }
    }
}
