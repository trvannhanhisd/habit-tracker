using HabitTracker.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Habits.Commands.DeleteHabit
{
    public class DeleteHabitCommandHandler : IRequestHandler<DeleteHabitCommand, int>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteHabitCommandHandler> _logger;

        public DeleteHabitCommandHandler(
            IHabitRepository habitRepository,
            ILogger<DeleteHabitCommandHandler> logger)
        {
            _habitRepository = habitRepository;
            _unitOfWork = habitRepository.UnitOfWork;
            _logger = logger;
        }

        public async Task<int> Handle(DeleteHabitCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete Habit with Id {HabitId}", request.HabitId);

            var habit = await _habitRepository.GetHabitByIdAsync(request.HabitId);
            if (habit == null)
            {
                _logger.LogWarning("Habit with Id {HabitId} not found", request.HabitId);
                throw new KeyNotFoundException($"Habit with Id {request.HabitId} not found.");
            }

            await _habitRepository.DeleteHabitAsync(request.HabitId);
            var affected = await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully deleted Habit {HabitId}, rows affected = {Affected}", request.HabitId, affected);

            return affected;
        }
    }
}
