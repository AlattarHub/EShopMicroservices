using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Middleware
{
    public static class ApplicationBuilderExtensions
    {
        public static IServiceCollection AddCustomObservability(
        this IServiceCollection services)
        {
            services.AddHealthChecks();

            return services;
        }
    }
}
