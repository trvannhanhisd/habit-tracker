

namespace HabitTracker.Domain.Exceptions.Auth
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException()
            : base("Invalid or expired authentication token.")
        {
        }
    }
}
