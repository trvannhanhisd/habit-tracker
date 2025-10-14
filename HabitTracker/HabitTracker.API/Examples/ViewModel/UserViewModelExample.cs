using HabitTracker.API.Models;
using HabitTracker.Application.Features.Auth.Commands.Login;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Examples.ViewModel
{
    public class UserViewModelExample : IExamplesProvider<ApiResponse<UserViewModel>>
    {
        public ApiResponse<UserViewModel> GetExamples()
        {
            return new ApiResponse<UserViewModel>(
                new UserViewModel
                {
                    Id = 1,
                    UserName = "john_doe",
                    Email = "john.doe@example.com"
                },
                200
            );
        }
    }
}
