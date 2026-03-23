using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Features.Orders.Commands;

namespace Ordering.API.EventBusConsumers
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMediator _mediator;

        public BasketCheckoutConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var eventId = context.MessageId?.ToString();
            var command = new CreateOrderCommand
            {
                EventId = eventId!,
                UserName = context.Message.UserName,
                TotalPrice = context.Message.TotalPrice,
                FirstName = context.Message.FirstName,
                LastName = context.Message.LastName,
                EmailAddress = context.Message.EmailAddress,
                AddressLine = context.Message.AddressLine,
                Country = context.Message.Country,
                State = context.Message.State,
                ZipCode = context.Message.ZipCode,
                CardName = context.Message.CardName,
                CardNumber = context.Message.CardNumber,
                Expiration = context.Message.Expiration,
                CVV = context.Message.CVV,
                PaymentMethod = context.Message.PaymentMethod,
                
            };

            await _mediator.Send(command);
        }
    }
}
