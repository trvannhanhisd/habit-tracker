

namespace HabitTracker.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task SendPetEvolvedAsync(int userId, int newLevel);
    }
}
