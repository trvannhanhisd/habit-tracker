using Microsoft.AspNetCore.SignalR;

namespace HabitTracker.API.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier ?? "Anonymous";
            Console.WriteLine($"[SignalR] User {userId} connected!");
            await base.OnConnectedAsync();
        }
    }

}
