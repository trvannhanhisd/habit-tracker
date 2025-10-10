using HabitTracker.Application.Features.Habits.Queries.GetHabits;
using MediatR;


namespace HabitTracker.Application.Features.Habits.Queries.GetHabitById
{
    public class GetHabitByIdQuery : IRequest<HabitViewModel>
    {
        public int HabitId { get; set; }
    }
}
