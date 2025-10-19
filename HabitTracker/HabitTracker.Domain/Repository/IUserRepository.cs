using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;

namespace HabitTracker.Infrastructure.Repository
{
    public interface IUserRepository
    {
        IUnitOfWork UnitOfWork { get; }
        Task<List<User>> GetAllUserAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<int> UpdateUserAsync(User user);
    }
}