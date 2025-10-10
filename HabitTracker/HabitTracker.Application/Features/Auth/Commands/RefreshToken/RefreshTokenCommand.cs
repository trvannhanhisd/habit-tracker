using HabitTracker.Application.Features.Auth.Commands.Login;
using MediatR;


namespace HabitTracker.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<TokenResponseViewModel>
    {
        public int UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
