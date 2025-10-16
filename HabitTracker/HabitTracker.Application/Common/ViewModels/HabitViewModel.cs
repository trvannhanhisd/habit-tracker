using HabitTracker.Application.Common.Mappings;
using HabitTracker.Domain.Entity;
using static HabitTracker.Domain.Entity.Habit;

namespace HabitTracker.Application.Common.ViewModels
{
    public class HabitViewModel : IMapFrom<Habit>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public HabitFrequency Frequency { get; set; } 
        public HabitCategory Category { get; set; }
        public int CurrentStreak { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsArchived { get; set; } = false;
    }
}
