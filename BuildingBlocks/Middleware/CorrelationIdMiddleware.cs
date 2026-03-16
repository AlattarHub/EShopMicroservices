using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Text;


namespace BuildingBlocks.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string HeaderName = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string correlationId;

            if (context.Request.Headers.ContainsKey(HeaderName))
            {
                correlationId = context.Request.Headers[HeaderName]!;
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers[HeaderName] = correlationId;
            }

            context.Response.Headers[HeaderName] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}
