using HabitTracker.Application.Auth.Commands.Login;
using MediatR;


namespace HabitTracker.Application.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommand : IRequest<TokenResponseViewModel?>
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
