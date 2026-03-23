using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.Grpc.Protos;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Serilog;

using BuildingBlocks.Observability;

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
    .AddRedis(
        builder.Configuration["CacheSettings:ConnectionString"]!,
        name: "redis",
        timeout: TimeSpan.FromSeconds(3))
    .AddRabbitMQ(
        builder.Configuration["EventBusSettings:HostAddress"]!,
        name: "rabbitmq",
        timeout: TimeSpan.FromSeconds(3));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379";
});

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<DiscountGrpcService>();

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(o =>
{
    o.Address = new Uri("http://discount-grpc:8080");
});

builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
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

app.Run();
