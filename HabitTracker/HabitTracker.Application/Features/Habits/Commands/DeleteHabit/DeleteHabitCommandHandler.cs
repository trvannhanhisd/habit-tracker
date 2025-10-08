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

        public DeleteHabitCommandHandler(IHabitRepository habitRepository)
        {
            _habitRepository = habitRepository;
        }
        public async Task<int> Handle(DeleteHabitCommand request, CancellationToken cancellationToken)
        {
            return await _habitRepository.DeleteAsync(request.Id);
        }
    }
}
