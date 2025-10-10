using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using MediatR;


namespace HabitTracker.Application.Features.Habits.Queries.GetHabitsByUser
{
    public class GetHabitsByUserQuery : IRequest<List<HabitViewModel>>
    {
        public int UserId { get; set; }
    }
}
