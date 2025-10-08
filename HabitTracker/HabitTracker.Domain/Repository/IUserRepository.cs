using HabitTracker.Domain.Entity;

namespace HabitTracker.Infrastructure.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllUserAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<int> UpdateUserAsync(User user);
    }
}