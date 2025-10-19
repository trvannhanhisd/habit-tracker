using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Domain.Exceptions.Auth
{
    public class UsernameAlreadyExistsException : Exception
    {
        public UsernameAlreadyExistsException(string username)
            : base($"Username '{username}' is already taken.")
        {
        }
    }
}
