using HabitTracker.Domain.Entity;
using MediatR;

namespace HabitTracker.Domain.Events
{
    public class HabitCompletedEvent : INotification
    {
        public int UserId { get; }
        public int HabitId { get; }

        public HabitCompletedEvent(int userId, int habitId)
        {
            UserId = userId;
            HabitId = habitId;
        }
    }
}
