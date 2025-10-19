using HabitTracker.Application.Common.ViewModels;
using MediatR;


namespace HabitTracker.Application.Features.HabitLogs.Commands.MarkHabitDone
{
    public class MarkHabitDoneCommand : IRequest<HabitLogViewModel>
    {
        public int HabitId { get; set; }
        public DateTime Date { get; set; }
    }
}
