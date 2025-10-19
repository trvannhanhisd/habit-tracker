using HabitTracker.Application.Common.ViewModels;
using MediatR;

namespace HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogById
{
    public class GetHabitLogByIdQuery : IRequest<HabitLogViewModel>
    {
        public int HabitLogId { get; set; }
    }
}
