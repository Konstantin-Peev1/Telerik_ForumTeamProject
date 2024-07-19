using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Telerik_ForumTeamProject.Hubs
{
    public class UserCountHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> ChatRoomUsers = new ConcurrentDictionary<string, HashSet<string>>();

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var chatRoomId = httpContext.Request.Query["chatRoomId"].ToString();
            var userId = httpContext.Request.Cookies["UserId"];  // Assuming UserId is also stored in a cookie
            var sessionId = httpContext.Request.Cookies["SessionId"];

            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);

            UpdateUserCount(chatRoomId, userId, sessionId, 1);

            // Send initial user counts
            await Clients.Caller.SendAsync("InitialUserCounts", GetActiveUserCounts());

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            var chatRoomId = httpContext.Request.Query["chatRoomId"].ToString();
            var userId = httpContext.Request.Cookies["UserId"];
            var sessionId = httpContext.Request.Cookies["SessionId"];

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId);

            UpdateUserCount(chatRoomId, userId, sessionId, -1);

            await base.OnDisconnectedAsync(exception);
        }

        private void UpdateUserCount(string chatRoomId, string userId, string sessionId, int change)
        {
            var userKey = $"{userId}:{sessionId}";
            if (!ChatRoomUsers.ContainsKey(chatRoomId))
            {
                ChatRoomUsers[chatRoomId] = new HashSet<string>();
            }

            if (change > 0)
            {
                ChatRoomUsers[chatRoomId].Add(userKey);
            }
            else
            {
                ChatRoomUsers[chatRoomId].Remove(userKey);
            }

            var userCount = ChatRoomUsers[chatRoomId].Count;
            Clients.Group(chatRoomId).SendAsync("UpdateUserCount", chatRoomId, userCount);
        }

        private IDictionary<string, int> GetActiveUserCounts()
        {
            var userCounts = new Dictionary<string, int>();
            foreach (var chatRoom in ChatRoomUsers)
            {
                userCounts[chatRoom.Key] = chatRoom.Value.Count;
            }
            return userCounts;
        }
    }
}
