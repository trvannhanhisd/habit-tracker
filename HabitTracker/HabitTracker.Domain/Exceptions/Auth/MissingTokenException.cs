using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Domain.Exceptions.Auth
{
    public class MissingTokenException : Exception
    {
        public MissingTokenException()
            : base("No authentication token provided.")
        {

        }
    }
}
