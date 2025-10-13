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

                var habits = await _habitRepository.GetHabitsWithoutLogForDateAsync(today);

                if (!habits.Any())
                {
                    _logger.LogInformation("Dont have any missed habit today ({Date})", today);
                    return Unit.Value;
                }

                foreach (var habit in habits)
                {
                    var habitLog = new HabitLog() {HabitId = habit.Id, Date = today, IsCompleted = false};
                    await _habitLogRepository.CreateHabitLogAsync(habitLog);
                }

                _logger.LogInformation("Created {Count} HabitLog IsCompleted = false for {Date}", habits.Count, today);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when creating Missed HabitLog");
                throw;
            }
        }

    }
}
