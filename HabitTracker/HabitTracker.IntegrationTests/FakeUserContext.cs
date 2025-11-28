using HabitTracker.Domain.Services;


namespace HabitTracker.IntegrationTests
{
    public class FakeUserContext : IUserContext
    {
        public int GetUserId() => 1;
        public string? GetUserEmail() => "test@example.com";
    }
}
