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

        public async Task<Habit?> GetHabitByUserIdAsync(int userId, int habitId)
        {
            return await _context.Habits
                .Where(h => h.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<Habit> CreateHabitAsync(Habit habit)
        {
            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();
            return habit;
        }

        public async Task<int> UpdateHabitAsync(Habit habit)
        {
            _context.Habits.Update(habit);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> ArchiveHabitAsync(int habitId)
        {
            var habit =  await _context.Habits.FindAsync(habitId);

            if (habit != null)
            {
                habit.IsArchived = true;
            }
            
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteHabitAsync(int id)
        {
            var habit = await GetHabitByIdAsync(id);
            if (habit != null)
            {
                _context.Habits.Remove(habit);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }


    }
}
