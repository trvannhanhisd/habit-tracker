using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Domain.Exceptions.Habit
{
    public class HabitNotFoundException : Exception
    {
        public HabitNotFoundException(string message) : base(message) { }
        public HabitNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
