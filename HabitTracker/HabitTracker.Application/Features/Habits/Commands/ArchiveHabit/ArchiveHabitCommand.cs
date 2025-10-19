using MediatR;

namespace HabitTracker.Application.Features.Habits.Commands.ArchiveHabit
{
    public class ArchiveHabitCommand : IRequest<int>
    {
        public int HabitId { get; set; }
    }
}
