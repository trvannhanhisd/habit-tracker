using HabitTracker.Domain.Entity;

namespace HabitTracker.Domain.Repository
{
    public interface IAuthRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User?> LoginUserAsync(string username);
        Task<User> RegisterUserAsync(User user);
        Task<bool> UserExistsAsync(string username);
    }
}