using HabitTracker.Domain.Entity;
using HabitTracker.Domain.Repository;
using HabitTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace HabitTracker.Infrastructure.Repository
{
    public class HabitLogRepository : IHabitLogRepository
    {
        private readonly HabitDbContext _context;

        public HabitLogRepository(HabitDbContext context)
        {
            _context = context;
        }

        public async Task<HabitLog?> GetHabitLogByIdAsync(int id)
        {
            return await _context.HabitLogs.FindAsync(id);
        }

        public async Task<List<HabitLog>> GetAllHabitAsync()
        {
            return await _context.HabitLogs.ToListAsync();
        }

        public async Task<List<HabitLog>> GetAllHabitLogsByHabitIdAsync(int habitId)
        {
            return await _context.HabitLogs
                .Where(h => h.HabitId == habitId)
                .ToListAsync();
        }

        public async Task<HabitLog?> GetHabitLogByHabitIdAndDateAsync(int habitId, DateTime date)
        {
            return await _context.HabitLogs
                .Where(h => h.HabitId == habitId && h.Date.Date == date.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<HabitLog> CreateHabitLogAsync(HabitLog habitLog)
        {
            _context.HabitLogs.Add(habitLog);
            await _context.SaveChangesAsync();
            return habitLog;
        }




    }
}
