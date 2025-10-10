using HabitTracker.Domain.Exceptions.Auth;
using System.Security.Claims;

namespace HabitTracker.Infrastructure.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new MissingTokenException();
            }
            return string.IsNullOrEmpty(userId) ? 0 : int.Parse(userId);
        }

        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(role))
            {
                throw new MissingTokenException();
            }

            return role;

        }
    }
}
