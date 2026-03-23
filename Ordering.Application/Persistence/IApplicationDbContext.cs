using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Application.Contracts.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<OutboxMessage> OutboxMessages { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
