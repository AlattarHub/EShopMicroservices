using BuildingBlocks.Middleware;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlocks.Observability
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomObservability(
        this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseHealthChecks("/health");

            return app;
        }
    }
}
