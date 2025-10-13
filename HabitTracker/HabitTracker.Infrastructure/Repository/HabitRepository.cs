using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository; 
using HabitTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabitTracker.Infrastructure.Repository
{
    public class HabitRepository : IHabitRepository
    {
        private readonly HabitDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public HabitRepository(HabitDbContext context)
        {
            _context = context;
        }

        public async Task<Habit?> GetHabitByIdAsync(int habitId)
        {
            return await _context.Habits.FindAsync(habitId);
        }

        public async Task<List<Habit>> GetAllHabitAsync()
        {
            return await _context.Habits.ToListAsync();
        }

        public async Task<List<Habit>> GetAllHabitsByUserIdAsync(int userId)
        {
            return await _context.Habits
                .Where(h => h.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Habit>> GetHabitsWithoutLogForDateAsync(DateTime date)
        {
            return await _context.Habits
                .Where(h => !_context.HabitLogs.Any(l => l.HabitId == h.Id && l.Date == date))
                .ToListAsync();
        }

        public async Task<Habit?> GetHabitByUserIdAsync(int userId, int habitId)
        {
            return await _context.Habits
                .Include(h => h.Logs)
                .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);
        }

        public Task<Habit> CreateHabitAsync(Habit habit)
        {
            _context.Habits.Add(habit);
            return Task.FromResult(habit);
        }

        public Task UpdateHabitAsync(Habit habit)
        {
            _context.Habits.Update(habit);
            return Task.CompletedTask;
        }

        public async Task ArchiveHabitAsync(int habitId)
        {
            var habit = await _context.Habits.FindAsync(habitId);
            if (habit != null)
            {
                habit.IsArchived = true;
            }
        }

        public async Task DeleteHabitAsync(int id)
        {
            var habit = await GetHabitByIdAsync(id);
            if (habit != null)
            {
                _context.Habits.Remove(habit);
            }
        }
    }
}
