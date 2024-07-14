using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Telerik_ForumTeamProject.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            var userName = Context.User.Identity.Name;
            await Clients.All.SendAsync("ReceiveMessage", userName, message);
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            var userName = Context.User.Identity.Name;
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", userName, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }

}
