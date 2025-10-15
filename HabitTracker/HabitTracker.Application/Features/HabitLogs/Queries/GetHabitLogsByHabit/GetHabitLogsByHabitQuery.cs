using HabitTracker.Application.Common.ViewModels;
using MediatR;


namespace HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs
{
    public class GetHabitLogsByHabitQuery : IRequest<List<HabitLogViewModel>>
    {
        public int HabitId { get; set; }
    }
}
