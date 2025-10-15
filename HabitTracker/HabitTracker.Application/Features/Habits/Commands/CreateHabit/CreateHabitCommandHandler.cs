using AutoMapper;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Exceptions.Auth;
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
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogWarning("Habit title is empty");
                throw new ArgumentException("Title cannot be empty");
            }

            var userId = _userContext.GetUserId();
            if (userId == 0)
            {
                _logger.LogWarning("User not authenticated for creating habit");
                throw new InvalidTokenException("User token is invalid or expired.");
            }

            try
            {
                var habit = new Habit()
                {
                    UserId = userId,
                    Title = request.Title,
                    Description = request.Description,
                    Frequency = request.Frequency
                };

                var result = await _habitRepository.CreateHabitAsync(habit);
                await _habitRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                return _mapper.Map<HabitViewModel>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating habit for user {userId}: {ex.Message}");
                throw;
            }
        }
    }
}
