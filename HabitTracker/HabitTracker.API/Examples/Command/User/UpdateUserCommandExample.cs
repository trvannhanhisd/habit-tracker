using HabitTracker.Application.Features.Users.Commands.UpdateUser;
using Swashbuckle.AspNetCore.Filters;

namespace HabitTracker.API.Examples.Command.User
{
    public class UpdateUserCommandExample : IExamplesProvider<UpdateUserCommand>
    {
        public UpdateUserCommand GetExamples()
        {
            return new UpdateUserCommand
            {
                Id = 1,
                UserName = "john_doe_updated",
                Email = "john.doe.updated@example.com"
            };
        }
    }
}
