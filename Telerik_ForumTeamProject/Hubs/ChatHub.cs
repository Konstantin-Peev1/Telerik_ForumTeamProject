using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var chatRoomId = httpContext.Request.Query["chatRoomId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            var chatRoomId = httpContext.Request.Query["chatRoomId"];
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(int chatRoomId, int userId, string userName, string message)
        {
            // Validate parameters
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(message) || chatRoomId <= 0 || userId <= 0)
            {
                throw new System.ArgumentException("Invalid parameters.");
            }

            // Save message to database
            _chatService.AddMessage(chatRoomId, userId, userName, message);

            // Notify clients in the same chat room
            await Clients.Group(chatRoomId.ToString()).SendAsync("ReceiveMessage", userName, message);
        }

    }
}