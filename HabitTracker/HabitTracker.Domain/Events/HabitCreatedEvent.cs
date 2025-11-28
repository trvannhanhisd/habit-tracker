using HabitTracker.Domain.Entity;
using MediatR;

namespace HabitTracker.Domain.Events
{
    public class HabitCreatedEvent : INotification
    {
        public int HabitId { get; }
        public int UserId { get; }
        public Habit.HabitCategory Category { get; }

        public HabitCreatedEvent(int habitId, int userId, Habit.HabitCategory category)
        {
            HabitId = habitId;
            UserId = userId;
            Category = category;
        }
    }
}