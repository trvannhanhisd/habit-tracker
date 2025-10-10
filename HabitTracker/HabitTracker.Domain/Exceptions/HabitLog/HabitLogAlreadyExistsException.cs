using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Domain.Exceptions.HabitLog
{
    public class HabitLogAlreadyExistsException : Exception
    {
        public HabitLogAlreadyExistsException(string message) : base(message) { }
        public HabitLogAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
