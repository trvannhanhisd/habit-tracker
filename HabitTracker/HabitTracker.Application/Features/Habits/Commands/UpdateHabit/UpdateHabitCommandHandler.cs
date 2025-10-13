
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using MediatR;


namespace HabitTracker.Application.Features.Habits.Commands.UpdateHabit
{
    public class UpdateHabitCommandHandler : IRequestHandler<UpdateHabitCommand, int>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateHabitCommandHandler(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
            _unitOfWork = habitRepository.UnitOfWork;
        }
        public async Task<int> Handle(UpdateHabitCommand request, CancellationToken cancellationToken)
        {
            var habit = new Habit { Id = request.Id, UserId = request.UserId, Description = request.Description, Frequency = request.Frequency, IsArchived = request.IsArchived };
            await _habitRepository.UpdateHabitAsync(habit);
            return await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
    