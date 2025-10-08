using HabitTracker.Domain.Entity;

namespace HabitTracker.Domain.Repository
{
    public interface IAuthRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> LoginUserAsync(string username);
        Task<User> RegisterUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> UserExistsAsync(string username);
    }
}