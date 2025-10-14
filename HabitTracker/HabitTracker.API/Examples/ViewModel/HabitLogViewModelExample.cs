using HabitTracker.API.Models;
using HabitTracker.Application.Features.HabitLogs.Queries.GetHabitLogs;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Examples.ViewModel
{
    public class HabitLogViewModelExample : IExamplesProvider<ApiResponse<HabitLogViewModel>>
    {
        public ApiResponse<HabitLogViewModel> GetExamples()
        {
            return new ApiResponse<HabitLogViewModel>(
                new HabitLogViewModel
                {
                    Id = 1,
                    HabitId = 1,
                    Date = DateTime.UtcNow,
                    IsCompleted = true
                },
                200
            );
        }
    }
}
