using AutoMapper;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using HabitTracker.Domain.Repository;
using MediatR;

namespace HabitTracker.Application.Features.Habits.Queries.GetHabitsByUser
{
    public class GetHabitsByUserQueryHandler : IRequestHandler<GetHabitsByUserQuery, List<HabitViewModel>>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IMapper _mapper;

        public GetHabitsByUserQueryHandler(IHabitRepository habitRepository, IMapper mapper)
        {
            _habitRepository = habitRepository;
            _mapper = mapper;
        }
        public async Task<List<HabitViewModel>> Handle(GetHabitsByUserQuery request, CancellationToken cancellationToken)
        {
            var habits = await _habitRepository.GetAllHabitsByUserIdAsync(request.UserId);

            var habitList = _mapper.Map<List<HabitViewModel>>(habits);

            return habitList;
        }
    }
}
