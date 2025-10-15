

namespace HabitTracker.Domain.Exceptions.Auth
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string v)
            : base("Invalid or expired authentication token.")
        {
        }
    }
}
