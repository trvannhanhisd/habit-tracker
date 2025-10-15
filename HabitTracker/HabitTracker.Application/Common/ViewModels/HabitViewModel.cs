using HabitTracker.Application.Common.Mappings;
using HabitTracker.Domain.Entity;

namespace HabitTracker.Application.Common.ViewModels
{
    public class HabitViewModel : IMapFrom<Habit>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string Frequency { get; set; } = "Daily"; // "Daily", "Weekly", etc.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsArchived { get; set; } = false;
    }
}
