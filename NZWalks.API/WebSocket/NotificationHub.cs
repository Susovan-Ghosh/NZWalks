using Microsoft.AspNetCore.SignalR;

namespace NZWalks.API.WebSocket
{
    public class NotificationHub : Hub
    {
        public async Task BroadcastMessageAsync(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
