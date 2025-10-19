using AutoMapper;
using HabitTracker.Application.Common.ViewModels;
using HabitTracker.Domain.Repository;
using MediatR;

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
