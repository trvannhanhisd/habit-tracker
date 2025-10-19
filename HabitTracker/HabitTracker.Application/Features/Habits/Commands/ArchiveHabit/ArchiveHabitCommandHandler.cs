
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Habits.Commands.ArchiveHabit
{
    public class ArchiveHabitCommandHandler : IRequestHandler<ArchiveHabitCommand, int>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ArchiveHabitCommandHandler> _logger;
        private readonly IUserContext _userContext;

        public ArchiveHabitCommandHandler(
            IHabitRepository habitRepository,
            ILogger<ArchiveHabitCommandHandler> logger,
            IUserContext userContext)
        {
            _habitRepository = habitRepository;
            _unitOfWork = habitRepository.UnitOfWork;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<int> Handle(ArchiveHabitCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            var habit = await _habitRepository.GetHabitByIdAsync(request.HabitId);

            if (habit == null || habit.UserId != userId)
            {
                _logger.LogWarning("Habit with ID {HabitId} not found or not owned by user {UserId}", request.HabitId, userId);
                throw new KeyNotFoundException($"Habit with ID {request.HabitId} not found.");
            }

            if (habit.IsArchived)
            {
                _logger.LogInformation("Habit {HabitId} already archived.", request.HabitId);
                return 0;
            }

            habit.IsArchived = true;
            await _habitRepository.UpdateHabitAsync(habit);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Habit {HabitId} archived successfully by user {UserId}", request.HabitId, userId);

            return result;
        }
    }
}
