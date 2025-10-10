using AutoMapper;
using HabitTracker.Domain.Repository;
using MediatR;


namespace HabitTracker.Application.Features.Habits.Queries.GetHabits
{
    public class GetHabitHandler : IRequestHandler<GetHabitQuery, List<HabitViewModel>>
    {
        private readonly IHabitRepository _habitRepository;
        private readonly IMapper _mapper;

        public GetHabitHandler(IHabitRepository habitRepository, IMapper mapper)
        {
            _habitRepository = habitRepository;
            _mapper = mapper;
        }
        public async Task<List<HabitViewModel>> Handle(GetHabitQuery request, CancellationToken cancellationToken)
        {
            var habits = await _habitRepository.GetAllHabitAsync();

            var habitList = _mapper.Map<List<HabitViewModel>>(habits);

            return habitList;
        }
    }
}
