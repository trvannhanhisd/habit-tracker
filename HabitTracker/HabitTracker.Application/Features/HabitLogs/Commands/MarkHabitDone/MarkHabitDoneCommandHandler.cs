using AutoMapper;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Domain.Exceptions.Habit;
using HabitTracker.Domain.Repository;
using HabitTracker.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HabitTracker.Application.Features.HabitLogs.Commands.MarkHabitDone
{
    public class MarkHabitDoneCommandHandler : IRequestHandler<MarkHabitDoneCommand, HabitLogViewModel>
    {
        private readonly IHabitLogRepository _habitLogRepository;
        private readonly IHabitRepository _habitRepository;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly ILogger<MarkHabitDoneCommandHandler> _logger;

        public MarkHabitDoneCommandHandler(
            IHabitLogRepository habitLogRepository,
            IHabitRepository habitRepository,
            IMapper mapper,
            IUserContext userContext,
            ILogger<MarkHabitDoneCommandHandler> logger)
        {
            _habitLogRepository = habitLogRepository;
            _habitRepository = habitRepository;
            _mapper = mapper;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<HabitLogViewModel> Handle(MarkHabitDoneCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetUserId();
            if (userId == 0)
            {
                _logger.LogWarning("User not authenticated for marking habit {HabitId} on {Date}", request.HabitId, request.Date);
                throw new UnauthorizedHabitAccessException("User not authenticated.");
            }

            _logger.LogInformation("Marking habit {HabitId} as done for user {UserId} on {Date}", request.HabitId, userId, request.Date);

            var habit = await _habitRepository.GetHabitByUserIdAsync(userId, request.HabitId);
            if (habit == null)
            {
                _logger.LogWarning("Habit {HabitId} not found for user {UserId}", request.HabitId, userId);
                throw new HabitNotFoundException($"Habit with ID {request.HabitId} not found for user {userId}.");
            }

            habit.MarkAsCompleted(request.Date);
            await _habitRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            var log = habit.Logs.FirstOrDefault(l => l.Date.Date == request.Date.Date);
            if (log == null)
            {
                _logger.LogError("No habit log created for habit {HabitId} on {Date}", request.HabitId, request.Date);
                throw new InvalidOperationException("HabitLog could not be created or retrieved.");
            }

            _logger.LogInformation("Habit {HabitId} marked as done successfully for user {UserId} on {Date}", request.HabitId, userId, request.Date);

            return _mapper.Map<HabitLogViewModel>(log);
        }
    }
}
