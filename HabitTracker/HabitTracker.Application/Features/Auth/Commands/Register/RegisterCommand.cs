using HabitTracker.Application.Features.Auth.Commands.Login;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<UserViewModel>
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
