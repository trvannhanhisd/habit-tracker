using HabitTracker.Application.Common.ViewModels;
using MediatR;


namespace HabitTracker.Application.Features.Habits.Queries.GetHabitsByUser
{
    public class GetHabitsByUserQuery : IRequest<List<HabitViewModel>>
    {
        public int UserId { get; set; }
    }
}
