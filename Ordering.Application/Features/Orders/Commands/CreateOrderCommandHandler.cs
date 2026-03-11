using MediatR;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace Ordering.Application.Features.Orders.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateOrderCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
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

            await _context.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
