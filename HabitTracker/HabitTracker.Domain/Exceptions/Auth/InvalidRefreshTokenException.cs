

namespace HabitTracker.Domain.Exceptions.Auth
{
    public class InvalidRefreshTokenException : Exception
    {
        public InvalidRefreshTokenException()
            : base("Invalid or expired refresh token.")
        {
        }
    }
}
