using HabitTracker.Application.Features.Auth.Commands.Login;
using MediatR;


namespace HabitTracker.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<List<UserViewModel>>
    {
    }
}
