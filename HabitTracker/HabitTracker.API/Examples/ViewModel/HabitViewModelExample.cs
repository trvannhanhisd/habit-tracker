using HabitTracker.API.Models;
using HabitTracker.Application.Features.Habits.Queries.GetHabits;
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
                    Frequency = "Daily"
                },
                200
            );
        }
    }
}
