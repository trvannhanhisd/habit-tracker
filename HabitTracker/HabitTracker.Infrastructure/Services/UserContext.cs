using HabitTracker.Domain.Services;
using HabitTracker.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace HabitTracker.Infrastructure.Services

{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.GetUserId() ?? 0;
        }
    }
}
