using MediatR;

namespace HabitTracker.Application.Features.Habits.Queries.GetHabits
{
    public class GetHabitQuery : IRequest<List<HabitViewModel>>
    {

    }
}
