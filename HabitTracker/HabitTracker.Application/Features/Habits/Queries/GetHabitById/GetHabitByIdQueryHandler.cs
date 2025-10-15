using AutoMapper;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Habits.Queries.GetHabitById
{
    public class GetHabitByIdQueryHandler : IRequestHandler<GetHabitByIdQuery, HabitViewModel>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetHabitByIdQueryHandler> _logger;

        public GetHabitByIdQueryHandler(
            IHabitRepository habitRepository,
            IMapper mapper,
            ILogger<GetHabitByIdQueryHandler> logger)
        {
            _habitRepository = habitRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<HabitViewModel> Handle(GetHabitByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.HabitId <= 0)
            {
                _logger.LogWarning("Invalid HabitId: {HabitId}", request.HabitId);
                throw new ArgumentException("HabitId must be greater than 0.");
            }

            _logger.LogInformation("Fetching habit with ID {HabitId}", request.HabitId);
            var habit = await _habitRepository.GetHabitByIdAsync(request.HabitId);

            if (habit == null)
            {
                _logger.LogWarning("Habit with ID {HabitId} not found", request.HabitId);
                throw new KeyNotFoundException($"Habit with ID {request.HabitId} not found.");
            }

            var habitViewModel = _mapper.Map<HabitViewModel>(habit);
            _logger.LogInformation("Successfully fetched habit {HabitId}", request.HabitId);

            return habitViewModel;
        }
    }
}
