using HabitTracker.Domain.Events;
using HabitTracker.Domain.Repository;
using HabitTracker.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.DomainEventHandler
{
    public class HabitMissedEventHandler : INotificationHandler<HabitMissedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HabitMissedEventHandler> _logger;

        public HabitMissedEventHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<HabitMissedEventHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(HabitMissedEvent notification, CancellationToken cancellationToken)
        {

            var user = await _userRepository.GetUserByIdAsync(notification.UserId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found when handling HabitMissedEvent", notification.UserId);
                return;
            }

            user.StreakCount = 0;

            _logger.LogInformation("Updated streak for User {UserId} after missing habit {HabitId}.",
                notification.UserId, notification.HabitId);
        }
    }
}
