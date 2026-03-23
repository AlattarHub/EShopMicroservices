using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;
using System.Text.Json;

namespace Ordering.API.BackgroundServices
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OutboxProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<OrderingContext>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var messages = await context.OutboxMessages
                    .Where(x => x.ProcessedOn == null)
                    .ToListAsync(stoppingToken);

                foreach (var message in messages)
                {
                    var order = JsonSerializer.Deserialize<Order>(message.Content);

                    await publishEndpoint.Publish(order);

                    message.ProcessedOn = DateTime.UtcNow;
                }

                await context.SaveChangesAsync(stoppingToken);

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
