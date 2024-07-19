using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Repositories.Contracts
{
    public interface IChatRepository
    {
        List<ChatRoom> GetActiveChats();
        ChatRoom GetChatRoom(int id);
        void AddMessage(ChatMessage message);
        void AddChatRoom(ChatRoom chatRoom);

    }
}
