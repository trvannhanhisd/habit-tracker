
using HabitTracker.Domain.Repository;
using MediatR;

namespace HabitTracker.Application.Features.Habits.Commands.ArchiveHabit
{
    public class ArchiveHabitCommandHandler : IRequestHandler<ArchiveHabitCommand, int>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ArchiveHabitCommandHandler(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
            _unitOfWork = habitRepository.UnitOfWork;
        }
        public async Task<int> Handle(ArchiveHabitCommand request, CancellationToken cancellationToken)
        {
            await _habitRepository.ArchiveHabitAsync(request.HabitId);
            return await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
