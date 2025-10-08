using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Domain.Entity
{
    public class Habit
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
    }
}
