using HabitTracker.Application.Features.Habits.Commands.CreateHabit;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Examples.Command.Habit
{
    public class CreateHabitCommandExample : IExamplesProvider<CreateHabitCommand>
    {
        public CreateHabitCommand GetExamples()
        {
            return new CreateHabitCommand
            {
                Title = "Drink Water 💧",
                Description = "Uống 2 lít nước mỗi ngày để khỏe da và cơ thể",
                Frequency = "Daily"
            };
        }
    }
}
