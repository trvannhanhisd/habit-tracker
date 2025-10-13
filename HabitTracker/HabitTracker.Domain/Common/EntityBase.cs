
using MediatR;

namespace HabitTracker.Domain.Common
{
    public abstract class EntityBase
    {
        private readonly List<INotification> _domainEvents = new();
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(INotification eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
