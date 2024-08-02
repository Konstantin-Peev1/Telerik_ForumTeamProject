using Microsoft.EntityFrameworkCore;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationContext applicationContext;

        public ChatRepository(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }

        public void AddMessage(ChatMessage message)
        {
            applicationContext.ChatMessages.Add(message);
            applicationContext.SaveChanges();
        }

        public List<ChatRoom> GetActiveChats()
        {
            return applicationContext.ChatRooms.Include(c => c.Messages).Include(c => c.Creator).ToList();
        }

        public ChatRoom GetChatRoom(int id)
        {
            return applicationContext.ChatRooms
                .Include(c => c.Messages.OrderBy(m => m.Created))
                .Include(c => c.Creator)
                    .ThenInclude(u => u.ChatRooms)
                .FirstOrDefault(c => c.Id == id);
        }

        public void AddChatRoom(ChatRoom chatRoom)
        {
            applicationContext.ChatRooms.Add(chatRoom);
           // chatRoom.Creator.ChatRooms.Add(chatRoom);
            applicationContext.SaveChanges();
        }
        public void DeleteChatRoom(ChatRoom chatRoom)
        {
            applicationContext.ChatRooms.Remove(chatRoom);
          //  chatRoom.Creator.ChatRooms.Remove(chatRoom);
            applicationContext.SaveChanges();
        }
        public void DeleteChatRoomIfOld(int chatRoomId)
        {
            var chatRoom = GetChatRoom(chatRoomId);
           
                chatRoom.Creator.ChatRooms.Remove(chatRoom);
                applicationContext.ChatRooms.Remove(chatRoom);
                applicationContext.SaveChanges();
            
        }
    }
}
