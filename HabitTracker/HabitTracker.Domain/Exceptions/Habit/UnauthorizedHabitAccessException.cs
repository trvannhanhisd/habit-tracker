using System;


namespace HabitTracker.Domain.Exceptions.Habit
{
    public class UnauthorizedHabitAccessException : Exception
    {
        public UnauthorizedHabitAccessException(string message) : base(message) { }
        public UnauthorizedHabitAccessException(string message, Exception innerException) : base(message, innerException) { }
    }
}
