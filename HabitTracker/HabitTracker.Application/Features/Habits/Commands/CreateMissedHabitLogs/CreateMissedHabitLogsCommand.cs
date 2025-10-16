using MediatR;
using static HabitTracker.Domain.Entity.Habit;


namespace HabitTracker.Application.Features.Habits.Commands.CreateMissedHabitLogs
{
    public class CreateMissedHabitLogsCommand : IRequest<Unit>
    {
        public HabitFrequency Frequency { get; set; } = HabitFrequency.Daily;

        public CreateMissedHabitLogsCommand()
        {
        }

        public CreateMissedHabitLogsCommand(HabitFrequency frequency)
        {
            Frequency = frequency;
        }
    }
}
