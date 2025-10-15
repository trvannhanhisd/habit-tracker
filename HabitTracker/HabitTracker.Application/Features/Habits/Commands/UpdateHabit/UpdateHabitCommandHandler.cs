using AutoMapper;
using HabitTracker.Domain.Exceptions.Auth;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.Habits.Commands.UpdateHabit
{
    public class UpdateHabitCommandHandler : IRequestHandler<UpdateHabitCommand, int>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateHabitCommandHandler> _logger;

        public UpdateHabitCommandHandler(
            IHabitRepository habitRepository,
            IMapper mapper,
            IUserContext userContext,
            ILogger<UpdateHabitCommandHandler> logger)
        {
            _habitRepository = habitRepository;
            _unitOfWork = habitRepository.UnitOfWork;
            _mapper = mapper;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateHabitCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (userId <= 0)
                throw new InvalidTokenException("User token is invalid or expired.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.Frequency))
                throw new ArgumentException("Frequency cannot be empty.");

            try
            {
                var habit = await _habitRepository.GetHabitByIdAsync(request.Id);
                if (habit == null)
                    throw new KeyNotFoundException("Habit not found.");

                if (habit.UserId != userId)
                    throw new UnauthorizedAccessException("You do not have permission to edit this habit.");

                // Update fields
                habit.Title = request.Title;
                habit.Description = request.Description;
                habit.Frequency = request.Frequency;
                habit.IsArchived = request.IsArchived;

                await _habitRepository.UpdateHabitAsync(habit);

                return await _habitRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating habit with ID {request.Id}: {ex.Message}");
                throw;
            }
        }
    }
}
