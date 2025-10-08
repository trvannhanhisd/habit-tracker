using HabitTracker.Application.Habits.Queries.GetHabits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Auth.Commands.Login
{
    public class LoginCommand : IRequest<TokenResponseViewModel?>
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
