using MediatR;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Application.Features.Orders.Queries
{
    public class GetOrdersList : IRequest<List<Order>>
    {
        public string UserName { get; set; } = default!;
    }
}
