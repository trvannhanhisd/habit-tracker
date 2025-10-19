using HabitTracker.Application.Common.Mappings;
using HabitTracker.Domain.Entity;


namespace HabitTracker.Application.Common.ViewModels
{
    public class HabitLogViewModel : IMapFrom<HabitLog>
    {
        public int Id { get; set; }
        public int HabitId { get; set; }
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
