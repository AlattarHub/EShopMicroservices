using BuildingBlocks.Observability;
using HealthChecks.UI.Client;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Ordering.API.BackgroundServices;
using Ordering.API.EventBusConsumers;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Features.Orders.Commands;
using Ordering.Infrastructure.Persistence;
using Polly;
using RabbitMQ.Client;
using Serilog;
using System.Reflection;


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMicroserviceObservability(builder.Configuration);
builder.Host.UseSerilog();
builder.Services.AddHealthChecks()
    .AddSqlServer(
        builder.Configuration.GetConnectionString("OrderingConnectionString")!,
        name: "sqlserver",
        timeout: TimeSpan.FromSeconds(3))
    .AddRabbitMQ(
        builder.Configuration["EventBusSettings:HostAddress"]!,
        name: "rabbitmq",
        timeout: TimeSpan.FromSeconds(3));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);
builder.Services.AddDbContext<OrderingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderingConnectionString")));
builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetService<OrderingContext>());
builder.Services.AddHostedService<OutboxProcessor>();
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<BasketCheckoutConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("basket-checkout-queue", c =>
        {
            c.ConfigureConsumer<BasketCheckoutConsumer>(ctx);

            // 🔥 Retry Policy
            c.UseMessageRetry(r =>
            {
                r.Interval(3, TimeSpan.FromSeconds(5));
            });
        });
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCustomObservability();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});


// Migration عند بداية التشغيل عشان الداتا بيز ما تروحش مع بداية ال  Container
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    try
//    {
//        var context = services.GetRequiredService<OrderingContext>();
//        context.Database.Migrate();
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex.Message);
//    }
//}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<OrderingContext>>();
    var context = services.GetRequiredService<OrderingContext>();

    try
    {
        logger.LogInformation("Migrating database associated with context OrderingContext");

        // استخدام Polly لعمل Retry في حال كان السيرفر لا يزال يبدأ
        var retry = Policy.Handle<SqlException>()
             .WaitAndRetry(
                 retryCount: 5,
                 sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                 onRetry: (exception, timeSpan, retry, ctx) =>
                 {
                     logger.LogWarning($"Retry {retry} due to {exception.Message}");
                 });

        retry.Execute(() => context.Database.Migrate());

        logger.LogInformation("Migrated database associated with context OrderingContext");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database used on context OrderingContext");
    }
}
    app.Run();
