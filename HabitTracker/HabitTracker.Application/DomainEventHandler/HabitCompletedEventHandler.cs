
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
        private readonly IHabitRepository _habitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<HabitCompletedEventHandler> _logger;

        public HabitCompletedEventHandler(
            IUserRepository userRepository,
            IHabitRepository habitRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            ILogger<HabitCompletedEventHandler> logger)
        {
            _userRepository = userRepository;
            _habitRepository = habitRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
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

            var habit = await _habitRepository.GetHabitByIdAsync(notification.HabitId);
            if(habit != null)
            {
                habit.CurrentStreak += 1;
            }

            // Lấy tất cả habit active của user
            var habits = await _habitRepository.GetAllHabitsByUserIdAsync(notification.UserId);
            var today = DateTime.UtcNow.Date;

            // Kiểm tra xem tất cả habit hôm nay đã hoàn thành chưa
            var allDone = habits.All(h =>
                h.Logs.Any(l => l.Date.Date == today && l.IsCompleted)
            );

            if (allDone) {
                user.StreakCount += 1;
            }

            _logger.LogInformation("User {UserId} hoàn thành tất cả habit hôm nay -> streak = {StreakCount}",
                user.Id, user.StreakCount);

            //await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
