using HabitTracker.API.Models;
using HabitTracker.Application.Common.ViewModels;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Examples.ViewModel
{
    public class HabitViewModelExample : IExamplesProvider<ApiResponse<HabitViewModel>>
    {
        public ApiResponse<HabitViewModel> GetExamples()
        {
            return new ApiResponse<HabitViewModel>(
                new HabitViewModel
                {
                    Id = 1,
                    Title = "Drink Water 💧",
                    Description = "Uống 2 lít nước mỗi ngày",
                    Frequency = Domain.Entity.Habit.HabitFrequency.Daily
                },
                200
            );
        }
    }
}
