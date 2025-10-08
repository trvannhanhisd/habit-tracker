using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Application.Auth.Commands.Login
{
    public class TokenResponseViewModel
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
