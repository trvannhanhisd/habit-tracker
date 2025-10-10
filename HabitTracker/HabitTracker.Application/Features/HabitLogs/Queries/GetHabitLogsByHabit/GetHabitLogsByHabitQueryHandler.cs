using AutoMapper;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Domain.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs
{
    public class GetHabitLogsByHabitQueryHandler : IRequestHandler<GetHabitLogsByHabitQuery, List<HabitLogViewModel>>
    {
        private readonly IHabitLogRepository _habitLogRepository;
        private readonly IMapper _mapper;

        public GetHabitLogsByHabitQueryHandler(IHabitLogRepository habitLogRepository, IMapper mapper)
        {
            _habitLogRepository = habitLogRepository;
            _mapper = mapper;
        }
        public async Task<List<HabitLogViewModel>> Handle(GetHabitLogsByHabitQuery request, CancellationToken cancellationToken)
        {
            var habitLogs = await _habitLogRepository.GetAllHabitLogsByHabitIdAsync(request.HabitId);

            var habitLogList = _mapper.Map<List<HabitLogViewModel>> (habitLogs);

            return habitLogList;

        }
    }
}
