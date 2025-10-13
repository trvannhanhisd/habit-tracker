using HabitTracker.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Habits.Commands.DeleteHabit
{
    public class DeleteHabitCommandHandler : IRequestHandler<DeleteHabitCommand, int>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteHabitCommandHandler(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
            _unitOfWork = habitRepository.UnitOfWork;
        }
        public async Task<int> Handle(DeleteHabitCommand request, CancellationToken cancellationToken)
        {
            await _habitRepository.DeleteHabitAsync(request.HabitId);
            return await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
