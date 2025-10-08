using AutoMapper;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Habits.Queries.GetHabitById
{
    public class GetHabitByIdQueryHandler : IRequestHandler<GetHabitByIdQuery, HabitViewModel>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IMapper _mapper;

        public GetHabitByIdQueryHandler(IHabitRepository habitRepository, IMapper mapper)
        {
            _habitRepository = habitRepository;
            _mapper = mapper;
        }

        public async Task<HabitViewModel> Handle(GetHabitByIdQuery request, CancellationToken cancellationToken)
        {
            var habit = await _habitRepository.GetByIdAsync(request.HabitId);
            return _mapper.Map<HabitViewModel>(habit);
        }
    }
}
