
using HabitTracker.Domain.Repository;
using MediatR;

namespace HabitTracker.Application.Features.Habits.Commands.ArchiveHabit
{
    public class ArchiveHabitCommandHandler : IRequestHandler<ArchiveHabitCommand, int>
    {
        private readonly IHabitRepository _habitRepository;

        public ArchiveHabitCommandHandler(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
        }
        public async Task<int> Handle(ArchiveHabitCommand request, CancellationToken cancellationToken)
        {
            return await _habitRepository.ArchiveHabitAsync(request.HabitId);
        }
    }
}
