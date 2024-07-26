using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Services.Contracts
{
    public interface IChatService
    {
        List<ChatRoom> GetActiveChats();
        ChatRoom GetChatRoom(int id);
        void AddMessage(int chatRoomId, int userId, string userName, string message);
        void CreateChatRoom(string name, User user); 
        void DeleteChatRoom(int chatRoomId, User user);

    }
}
