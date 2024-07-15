using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Telerik_ForumTeamProject.Hubs
{
    public class OnlineUsersHub : Hub
    {
        private static ConcurrentDictionary<string, string> _onlineUsers = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            _onlineUsers.TryAdd(Context.ConnectionId, Context.ConnectionId);
            Clients.All.SendAsync("UpdateOnlineUsersCount", _onlineUsers.Count);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _onlineUsers.TryRemove(Context.ConnectionId, out _);
            Clients.All.SendAsync("UpdateOnlineUsersCount", _onlineUsers.Count);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
