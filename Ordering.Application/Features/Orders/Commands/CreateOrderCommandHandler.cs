using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;


namespace Ordering.Application.Features.Orders.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        public CreateOrderCommandHandler(IApplicationDbContext context, ILogger<CreateOrderCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var exists = await _context.Orders.AnyAsync(o => o.EventId == request.EventId, cancellationToken);

            if (exists)
            {
                _logger.LogWarning("Duplicate event detected: {EventId}", request.EventId);

                return 0; // أو ممكن ترجع Id قديم لو حابب
            }
            var order = new Order
            {
                EventId = request.EventId,
                UserName = request.UserName,
                TotalPrice = request.TotalPrice,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailAddress = request.EmailAddress,
                AddressLine = request.AddressLine,
                Country = request.Country,
                State = request.State,
                ZipCode = request.ZipCode,
                CardName = request.CardName,
                CardNumber = request.CardNumber,
                Expiration = request.Expiration,
                CVV = request.CVV,
                PaymentMethod = request.PaymentMethod
            };

            _context.Orders.Add(order);

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "OrderCreated",
                Content = JsonSerializer.Serialize(order),
                OccurredOn = DateTime.UtcNow
            };

            _context.OutboxMessages.Add(outboxMessage);

            await _context.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
