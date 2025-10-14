using HabitTracker.API.Models;
using HabitTracker.Application.Features.Auth.Commands.Login;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Examples.ViewModel
{
    public class ListUserViewModelExample : IExamplesProvider<ApiResponse<List<UserViewModel>>>
    {
        public ApiResponse<List<UserViewModel>> GetExamples()
        {
            return new ApiResponse<List<UserViewModel>>(
                new List<UserViewModel>
                {
                    new UserViewModel
                    {
                        Id = 1,
                        UserName = "john_doe",
                        Email = "john.doe@example.com"
                    },
                    new UserViewModel
                    {
                        Id = 2,
                        UserName = "jane_smith",
                        Email = "jane.smith@example.com"
                    }
                },
                200
            );
        }
    }
}
