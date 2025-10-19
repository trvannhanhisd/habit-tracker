using AutoMapper;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Habits.Commands.CreateMissedHabitLogs
{
    public class CreateMissedHabitLogsCommandHandler : IRequestHandler<CreateMissedHabitLogsCommand, Unit>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IHabitLogRepository _habitLogRepository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly ILogger<CreateMissedHabitLogsCommandHandler> _logger;

        public CreateMissedHabitLogsCommandHandler(
            IHabitRepository habitRepository,
            IHabitLogRepository habitLogRepository,
            IMapper mapper,
            IUserContext userContext,
            ILogger<CreateMissedHabitLogsCommandHandler> logger)
        {
            _habitRepository = habitRepository;
            _habitLogRepository = habitLogRepository;
            _mapper = mapper;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateMissedHabitLogsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var today = DateTime.Today;

                _logger.LogInformation("Checking missed habits for date {Date}", today);

                var habits = await _habitRepository.GetHabitsWithoutLogForDateAsync(today);

                if (habits == null || !habits.Any())
                {
                    _logger.LogInformation("No missed habits found for {Date}", today);
                    return Unit.Value;
                }

                var filteredHabits = habits
                .Where(h => h.Frequency == request.Frequency)
                .ToList();

                if (!filteredHabits.Any())
                {
                    _logger.LogInformation("No {Frequency} habits found for {Date}", request.Frequency, today);
                    return Unit.Value;
                }

                foreach (var habit in habits)
                {
                    habit.MarkAsMissed(today);
                }

                await _habitRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created {Count} missed HabitLogs for {Date}", habits.Count, today);
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when creating missed habit logs");
                throw;
            }
        }
    }
}
