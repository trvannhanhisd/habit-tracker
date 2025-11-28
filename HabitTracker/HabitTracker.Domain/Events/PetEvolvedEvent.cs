

using MediatR;

namespace HabitTracker.Domain.Events
{
    public class PetEvolvedEvent : INotification
    {
        public int UserId { get; }
        public int NewLevel { get; }

        public PetEvolvedEvent(int userId, int newLevel)
        {
            UserId = userId;
            NewLevel = newLevel;
        }
    }
}
