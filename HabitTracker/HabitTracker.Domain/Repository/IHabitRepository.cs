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
        Task<Habit> GetByIdAsync(int habitId);
        Task<Habit> CreateAsync(Habit habit);
        Task<int> UpdateAsync(Habit habit);
        Task<int> DeleteAsync(int habitId);

    }
}
