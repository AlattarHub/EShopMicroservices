using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderingContext : DbContext, IApplicationDbContext
    {
        public OrderingContext(DbContextOptions<OrderingContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
    }
}
