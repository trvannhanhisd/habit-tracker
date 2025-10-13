using MediatR;


namespace HabitTracker.Domain.Events
{
    public class HabitMissedEvent : INotification
    {
        public int UserId { get; }
        public int HabitId { get; }

        public HabitMissedEvent(int userId, int habitId)
        {
            UserId = userId;
            HabitId = habitId;
        }
    }
}
