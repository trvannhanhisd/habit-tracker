using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Domain.Exceptions.User
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(int userId)
            : base($"User with ID {userId} not found.")
        {
        }

        public UserNotFoundException(string username)
            : base($"User with username '{username}' not found.")
        {
        }
    }
}
