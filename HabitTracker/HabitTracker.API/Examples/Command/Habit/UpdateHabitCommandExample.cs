using HabitTracker.Application.Features.Habits.Commands.UpdateHabit;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Examples.Command.Habit
{
    public class UpdateHabitCommandExample : IExamplesProvider<UpdateHabitCommand>
    {
        public UpdateHabitCommand GetExamples()
        {
            return new UpdateHabitCommand
            {
                Id = 1,
                Title = "Drink Water 💧 (Updated)",
                Description = "Uống 2.5 lít nước mỗi ngày để khỏe mạnh",
                Frequency = "Daily"
            };
        }
    }
}
