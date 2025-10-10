using HabitTracker.Domain.Entity;

namespace HabitTracker.Domain.Repository
{
    public interface IHabitLogRepository
    {
        Task<HabitLog> CreateHabitLogAsync(HabitLog habitLog);
        Task<List<HabitLog>> GetAllHabitAsync();
        Task<List<HabitLog>> GetAllHabitLogsByHabitIdAsync(int habitId);
        Task<HabitLog?> GetHabitLogByHabitIdAndDateAsync(int habitId, DateTime date);
        Task<HabitLog?> GetHabitLogByIdAsync(int id);
    }
}