

using HabitTracker.Application.Common.Interfaces;
using HabitTracker.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.DomainEventHandler
{
    public class PetEvolvedEventHandler : INotificationHandler<PetEvolvedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<PetEvolvedEventHandler> _logger;

        public PetEvolvedEventHandler(INotificationService notificationService, ILogger<PetEvolvedEventHandler> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task Handle(PetEvolvedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.SendPetEvolvedAsync(notification.UserId, notification.NewLevel);

            _logger.LogInformation("🔔 Pet evolved: User {UserId}, Level {Level}",
                notification.UserId, notification.NewLevel);
        }
    }
}
