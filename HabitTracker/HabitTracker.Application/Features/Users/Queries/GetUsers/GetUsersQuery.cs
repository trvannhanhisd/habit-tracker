using HabitTracker.Application.Common.ViewModels;
using MediatR;


namespace HabitTracker.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<List<UserViewModel>>
    {
    }
}
