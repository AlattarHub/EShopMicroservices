using BuildingBlocks.Observability;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://seq:5341")
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// Add services to the container.
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddMicroserviceObservability(builder.Configuration);
builder.Host.UseSerilog();
builder.Services.AddHealthChecksUI(setup =>
{
    setup.AddHealthCheckEndpoint("Catalog API", "http://catalog-api:8080/health");
    setup.AddHealthCheckEndpoint("Basket API", "http://basket-api:8080/health");
    setup.AddHealthCheckEndpoint("Discount API", "http://discount-api:8080/health");
    setup.AddHealthCheckEndpoint("Ordering API", "http://ordering-api:8080/health");
})
.AddInMemoryStorage();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOcelot(builder.Configuration);


var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
   
}


app.UseRouting();
app.UseCustomObservability();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
endpoints.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
    endpoints.MapHealthChecksUI(options =>
    {
        options.UIPath = "/healthchecks-ui";
        options.ApiPath = "/healthchecks-api";
    });
    endpoints.MapControllers();
});


await app.UseOcelot();

app.Run();
