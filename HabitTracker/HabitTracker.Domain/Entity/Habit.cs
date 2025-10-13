using HabitTracker.Domain.Common;
using HabitTracker.Domain.Events;


namespace HabitTracker.Domain.Entity
{
    public class Habit : EntityBase
    {
        public int Id { get; set; }              // Primary key
        public int UserId { get; set; }          // FK tới User
        public string Title { get; set; } = "";   // Tên thói quen
        public string? Description { get; set; }  // Mô tả
        public string Frequency { get; set; } = "Daily"; // "Daily", "Weekly", etc.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsArchived { get; set; } = false; // Nếu user muốn tạm dừng habit

        public User? User { get; set; }
        public List<HabitLog> Logs { get; set; } = new List<HabitLog>();


        public void MarkAsCompleted(DateTime date)
        {
            if (Logs.Any(l => l.Date.Date == date.Date))
                return;

            Logs.Add(new HabitLog
            {
                Date = date,
                IsCompleted = true
            });

            AddDomainEvent(new HabitCompletedEvent(UserId, Id));
        }

        public void MarkAsMissed(DateTime date)
        {
            if (Logs.Any(l => l.Date.Date == date.Date))
                return;

            Logs.Add(new HabitLog
            {
                Date = date,
                IsCompleted = false
            });

            AddDomainEvent(new HabitMissedEvent(UserId, Id));
        }
    }
}
