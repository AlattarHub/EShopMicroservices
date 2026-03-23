using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Features.Orders.Queries;
using Ordering.Domain.Entities;


namespace Ordering.Application.Features.Orders.Commands
{
    public class GetOrdersListHandler : IRequestHandler<GetOrdersList, List<Order>>
    {
        private readonly IApplicationDbContext _context;

        public GetOrdersListHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> Handle(GetOrdersList request, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Where(o => o.UserName == request.UserName)
                .ToListAsync(cancellationToken);
        }
    }
}
