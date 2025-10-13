
using HabitTracker.Domain.Events;
using HabitTracker.Domain.Repository;
using HabitTracker.Infrastructure.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.DomainEventHandler
{
    public class HabitCompletedEventHandler : INotificationHandler<HabitCompletedEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HabitCompletedEventHandler> _logger;

        public HabitCompletedEventHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ILogger<HabitCompletedEventHandler> logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(HabitCompletedEvent notification, CancellationToken cancellationToken)
        {

            var user = await _userRepository.GetUserByIdAsync(notification.UserId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found when handling HabitCompletedEvent", notification.UserId);
                return;
            }

            user.StreakCount += 1;

            _logger.LogInformation("Updated streak for User {UserId} after completing habit {HabitId}.",
                notification.UserId, notification.HabitId);
        }
    }
}
