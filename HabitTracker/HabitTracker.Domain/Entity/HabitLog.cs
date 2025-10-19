

namespace HabitTracker.Domain.Entity
{
    public class HabitLog
    {
        public int Id { get; set; }              // Primary key
        public int HabitId { get; set; }         // FK tới Habit
        public DateTime Date { get; set; }        // Ngày check-in
        public bool IsCompleted { get; set; }     // True nếu đã hoàn thành
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Habit? Habit { get; set; }
    }
}
