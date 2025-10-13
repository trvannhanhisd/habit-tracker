using HabitTracker.Domain.Entity;

namespace HabitTracker.Domain.Repository
{
    public interface IHabitRepository
    {
        IUnitOfWork UnitOfWork { get; }
        Task<List<Habit>> GetAllHabitAsync();
        Task<Habit?> GetHabitByIdAsync(int habitId);
        Task<Habit> CreateHabitAsync(Habit habit);
        Task<List<Habit>> GetAllHabitsByUserIdAsync(int userId);
        Task<Habit?> GetHabitByUserIdAsync(int userId, int habitId);
        Task<List<Habit>> GetHabitsWithoutLogForDateAsync(DateTime date);
        Task DeleteHabitAsync(int id);
        Task ArchiveHabitAsync(int habitId);
        Task UpdateHabitAsync(Habit habit);
    }
}
