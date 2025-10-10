
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using MediatR;


namespace HabitTracker.Application.Features.Habits.Commands.UpdateHabit
{
    public class UpdateHabitCommandHandler : IRequestHandler<UpdateHabitCommand, int>
    {
        private readonly IHabitRepository _habitRepository;

        public UpdateHabitCommandHandler(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
        }
        public async Task<int> Handle(UpdateHabitCommand request, CancellationToken cancellationToken)
        {
            var habit = new Habit { Id = request.Id, UserId = request.UserId, Description = request.Description, Frequency = request.Frequency, IsArchived = request.IsArchived };
            return await _habitRepository.UpdateHabitAsync(habit);
        }
    }
}
