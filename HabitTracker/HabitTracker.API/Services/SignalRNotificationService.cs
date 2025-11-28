using HabitTracker.API.Hubs;
using HabitTracker.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace HabitTracker.API.Services
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendPetEvolvedAsync(int userId, int newLevel)
        {
            await _hubContext.Clients.User(userId.ToString())
                .SendAsync("PetEvolved", new { newLevel });
        }
    }
}
