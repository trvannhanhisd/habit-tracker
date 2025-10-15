using AutoMapper;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Habits.Queries.GetHabitsByUser
{
    public class GetHabitsByUserQueryHandler : IRequestHandler<GetHabitsByUserQuery, List<HabitViewModel>>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetHabitsByUserQueryHandler> _logger;
        private readonly IUserContext _userContext;

        public GetHabitsByUserQueryHandler(
            IHabitRepository habitRepository,
            IMapper mapper,
            ILogger<GetHabitsByUserQueryHandler> logger,
            IUserContext userContext)
        {
            _habitRepository = habitRepository;
            _mapper = mapper;
            _logger = logger;
            _userContext = userContext;
        }

        public async Task<List<HabitViewModel>> Handle(GetHabitsByUserQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId != 0 ? request.UserId : _userContext.GetUserId();

            if (userId <= 0)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", userId);
                throw new ArgumentException("Invalid user ID.");
            }

            _logger.LogInformation("Fetching habits for user {UserId}", userId);
            var habits = await _habitRepository.GetAllHabitsByUserIdAsync(userId);

            if (habits == null || !habits.Any())
            {
                _logger.LogInformation("No habits found for user {UserId}", userId);
                return new List<HabitViewModel>();
            }

            var habitList = _mapper.Map<List<HabitViewModel>>(habits);
            _logger.LogInformation("Successfully fetched {Count} habits for user {UserId}", habitList.Count, userId);

            return habitList;
        }
    }
}
