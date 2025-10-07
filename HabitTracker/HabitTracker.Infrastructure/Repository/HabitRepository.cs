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

        public async Task<Habit> GetByIdAsync(int id) // Giả sử method này
        {
            return await _context.Habits.FindAsync(id);
        }

        public async Task<List<Habit>> GetAllHabitAsync() // Thêm các method khác tương ứng
        {
            return await _context.Habits.ToListAsync();
        }

        public async Task<Habit> CreateAsync(Habit habit)
        {
            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();
            return habit;
        }

        public async Task<int> UpdateAsync(Habit habit)
        {
            _context.Habits.Update(habit);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(int id)
        {
            var habit = await GetByIdAsync(id);
            if (habit != null)
            {
                _context.Habits.Remove(habit);
                return await _context.SaveChangesAsync();
            }
            return 0;
        }

    }
}
