using AutoMapper;
using HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs;
using HabitTracker.Domain.Repository;
using MediatR;

namespace HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogById
{
    public class GetHabitLogByIdQueryHandler : IRequestHandler<GetHabitLogByIdQuery, HabitLogViewModel?>
    {
        private readonly IHabitLogRepository _habitLogRepository;
        private readonly IMapper _mapper;

        public GetHabitLogByIdQueryHandler(IHabitLogRepository habitLogRepository, IMapper mapper)
        {
            _habitLogRepository = habitLogRepository;
            _mapper = mapper;
        }

        public async Task<HabitLogViewModel?> Handle(GetHabitLogByIdQuery request, CancellationToken cancellationToken)
        {
            var habitLog = await _habitLogRepository.GetHabitLogByIdAsync(request.HabitLogId);
            return habitLog == null ? null : _mapper.Map<HabitLogViewModel>(habitLog);
        }
    }
}
