using HabitTracker.Application.Features.Habits.Commands.UpdateHabit;
using Swashbuckle.AspNetCore.Filters;
using static HabitTracker.Domain.Entity.Habit;

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
                Frequency = HabitFrequency.Daily,
                Category = HabitCategory.General
            };
        }
    }
}
