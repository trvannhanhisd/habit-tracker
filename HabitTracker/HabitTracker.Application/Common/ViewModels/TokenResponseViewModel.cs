

namespace HabitTracker.Application.Common.ViewModels
{
    public class TokenResponseViewModel
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
