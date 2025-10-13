using HabitTracker.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Infrastructure.Extensions
{
    public static class MediatorExtensions
    {
        /// <summary>
        /// Thu thập domain events từ ChangeTracker của context và publish qua MediatR.
        /// LƯU Ý: không clear events ở đây (hoặc clear ngay sau khi thu thập) để tránh re-publish.
        /// </summary>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext context, CancellationToken cancellationToken = default)
        {
            // Lấy các entity có domain events
            var domainEntities = context.ChangeTracker
                .Entries<EntityBase>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents != null && e.DomainEvents.Any())
                .ToList();

            // Lấy tất cả events hiện có (snapshot)
            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Clear domain events trên entity để tránh publish lại trong cùng quá trình
            domainEntities.ForEach(e => e.ClearDomainEvents());

            // Publish từng event
            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
