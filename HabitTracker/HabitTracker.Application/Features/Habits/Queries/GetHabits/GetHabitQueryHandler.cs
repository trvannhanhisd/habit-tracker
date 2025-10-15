using AutoMapper;
using HabitTracker.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Habits.Queries.GetHabits
{
    public class GetHabitHandler : IRequestHandler<GetHabitQuery, List<HabitViewModel>>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetHabitHandler> _logger;

        public GetHabitHandler(IHabitRepository habitRepository, IMapper mapper, ILogger<GetHabitHandler> logger)
        {
            _habitRepository = habitRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<HabitViewModel>> Handle(GetHabitQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching all habits");

                var habits = await _habitRepository.GetAllHabitAsync();

                if (habits == null || !habits.Any())
                {
                    _logger.LogInformation("No habits found");
                    return new List<HabitViewModel>();
                }

                var habitList = _mapper.Map<List<HabitViewModel>>(habits);

                _logger.LogInformation("Fetched {Count} habits", habitList.Count);
                return habitList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when fetching all habits: {Message}", ex.Message);
                throw;
            }
        }
    }
}
