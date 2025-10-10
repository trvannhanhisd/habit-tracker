using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Domain.Exceptions.Auth
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Invalid username or password.")
        {
        }
    }
}
