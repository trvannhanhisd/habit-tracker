using HabitTracker.API.Models;
using HabitTracker.Application.Common.ViewModels;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Examples.ViewModel
{
    public class ListHabitViewModelExample : IExamplesProvider<ApiResponse<List<HabitViewModel>>>
    {
        public ApiResponse<List<HabitViewModel>> GetExamples()
        {
            return new ApiResponse<List<HabitViewModel>>(
                new List<HabitViewModel>
                {
                    new HabitViewModel
                    {
                        Id = 1,
                        Title = "Drink Water 💧",
                        Description = "Uống 2 lít nước mỗi ngày",
                        Frequency = Domain.Entity.Habit.HabitFrequency.Daily,
                        Category = Domain.Entity.Habit.HabitCategory.General
                    },
                    new HabitViewModel
                    {
                        Id = 2,
                        Title = "Exercise 🏋️",
                        Description = "Tập thể dục 30 phút mỗi ngày",
                        Frequency = Domain.Entity.Habit.HabitFrequency.Daily,
                        Category = Domain.Entity.Habit.HabitCategory.General
                    }
                },
                200
            );
        }
    }
}
