using HabitTracker.Domain.Common;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using HabitTracker.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HabitTracker.Infrastructure.Data
{
    public class HabitDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;
        public HabitDbContext(DbContextOptions<HabitDbContext> options, IMediator mediator)
        : base(options)
        {
            _mediator = mediator;
        }
        public DbSet<Habit> Habits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<HabitLog> HabitLogs { get; set; }
        // Configure entity relationships and constraints here
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Apply configurations for your entities
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var strategy = Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                // If ambient transaction exists, just use it (avoid nested)
                if (Database.CurrentTransaction != null)
                {
                    // Publish domain events and then save
                    await _mediator.DispatchDomainEventsAsync(this, cancellationToken);
                    return await base.SaveChangesAsync(cancellationToken);
                }

                await using var txn = await Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // 1) Publish domain events so handlers can modify tracked entities
                    await _mediator.DispatchDomainEventsAsync(this, cancellationToken);

                    // 2) Persist all changes (including those made by handlers)
                    var result = await base.SaveChangesAsync(cancellationToken);

                    // 3) Commit transaction
                    await txn.CommitAsync(cancellationToken);
                    return result;
                }
                catch
                {
                    await txn.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        private async Task DispatchDomainEvents()
        {
            var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents != null && e.DomainEvents.Any())
                .ToList();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();

                foreach (var domainEvent in events)
                {
                    await _mediator.Publish(domainEvent);
                }
            }
        }
    }
}
