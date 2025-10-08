using AutoMapper;
using HabitTracker.Application.Habits.Queries.GetHabits;
using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Habits.Commands.CreateHabit
{
    public class CreateHabitCommandHandler : IRequestHandler<CreateHabitCommand, HabitViewModel>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IMapper _mapper;

        public CreateHabitCommandHandler(IHabitRepository habitRepository, IMapper mapper) 
        {
            _habitRepository = habitRepository;
            _mapper = mapper;
        }
        public async Task<HabitViewModel> Handle(CreateHabitCommand request, CancellationToken cancellationToken)
        {
            var habit = new Habit() { UserId = request.UserId ,Title = request.Title, Description = request.Description, Frequency = request.Frequency };
            var Result = await _habitRepository.CreateAsync(habit);
            return _mapper.Map<HabitViewModel>(Result);
        }
    }
}
