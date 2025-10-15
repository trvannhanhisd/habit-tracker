
using HabitTracker.Application.Common.ViewModels;
using MediatR;


namespace HabitTracker.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserViewModel>
    {
        public int UserId { get; set; }
    }
}
