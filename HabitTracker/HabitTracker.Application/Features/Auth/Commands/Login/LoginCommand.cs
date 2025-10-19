using HabitTracker.Application.Common.ViewModels;
using MediatR;

namespace HabitTracker.Application.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<TokenResponseViewModel>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
