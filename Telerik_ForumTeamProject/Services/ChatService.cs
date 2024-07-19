
using System;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Services
{
  
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public List<ChatRoom> GetActiveChats()
        {
            return _chatRepository.GetActiveChats();
        }

        public ChatRoom GetChatRoom(int id)
        {
            return _chatRepository.GetChatRoom(id);
        }

        public void AddMessage(int chatRoomId, int userId, string userName, string message)
        {
            var chatRoom = _chatRepository.GetChatRoom(chatRoomId);
            if (chatRoom != null)
            {
                var chatMessage = new ChatMessage
                {
                    UserId = userId,
                    UserName = userName,
                    Message = message,
                    Created = DateTime.UtcNow,
                    ChatRoomId = chatRoomId,
                    ChatRoom = chatRoom
                };

                _chatRepository.AddMessage(chatMessage);
            }
        }
        public void CreateChatRoom(string name)
        {
            var chatRoom = new ChatRoom
            {
                Name = name,
                Messages = new List<ChatMessage>()
            };
            _chatRepository.AddChatRoom(chatRoom);
        }
    }
}
