using HabitTracker.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Domain.Repository
{
    public interface IHabitRepository
    {
        Task<List<Habit>> GetAllHabitAsync();
        Task<Habit?> GetHabitByIdAsync(int habitId);
        Task<Habit> CreateHabitAsync(Habit habit);
        Task<int> UpdateHabitAsync(Habit habit);
        Task<int> DeleteHabitAsync(int habitId);
        Task<List<Habit>> GetAllHabitsByUserIdAsync(int userId);
        Task<int> ArchiveHabitAsync(int habitId);
        Task<Habit?> GetHabitByUserIdAsync(int userId, int habitId);
        Task<List<Habit>> GetHabitsWithoutLogForDateAsync(DateTime date);
    }
}
