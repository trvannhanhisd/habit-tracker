using AutoMapper;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Exceptions.Auth;
using HabitTracker.Domain.Exceptions.Habit;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Habits.Commands.CreateHabit
{
    public class CreateHabitCommandHandler : IRequestHandler<CreateHabitCommand, HabitViewModel>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly ILogger<CreateHabitCommandHandler> _logger;

        public CreateHabitCommandHandler(IHabitRepository habitRepository, IMapper mapper, 
                                        IUserContext userContext, ILogger<CreateHabitCommandHandler> logger)
        {
            _habitRepository = habitRepository;
            _mapper = mapper;
            _userContext = userContext;
            _logger = logger;
        }
        public async Task<HabitViewModel> Handle(CreateHabitCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (userId == 0)
            {
                _logger.LogWarning("User not authenticated for creating habit");
                throw new InvalidTokenException();
            }

            var habit = new Habit() { UserId = userId, Title = request.Title, Description = request.Description, Frequency = request.Frequency };
            var Result = await _habitRepository.CreateHabitAsync(habit);
            return _mapper.Map<HabitViewModel>(Result);
        }
    }
}
