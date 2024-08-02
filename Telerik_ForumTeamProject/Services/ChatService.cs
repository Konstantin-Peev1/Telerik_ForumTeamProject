
using System;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Exceptions;
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
        public void CreateChatRoom(string name, User user)
        {
            var chatRoom = new ChatRoom
            {
                UserId = user.ID,
                Name = name,
                Messages = new List<ChatMessage>(),
                Created = DateTime.UtcNow,
            };

            _chatRepository.AddChatRoom(chatRoom);
        }
        public void DeleteChatRoom(int chatRoomId, User user)
        {
            var chatRoom = _chatRepository.GetChatRoom(chatRoomId);
            if (chatRoom != null)
            {
                if (chatRoom.UserId == user.ID || user.IsAdmin)
                {
                    if ((DateTime.UtcNow - chatRoom.Created).TotalHours <= 24)
                    {
                        _chatRepository.DeleteChatRoom(chatRoom);
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException("User is not authorized to delete this chat room.");
                }
            }
            
        }
        public void DeleteChatRoomIFOld()
        {
            foreach (var chatRoom in GetActiveChats())
            {
               
                if (chatRoom != null && (DateTime.UtcNow - chatRoom.Created).TotalHours > 24)
                {
                    _chatRepository.DeleteChatRoomIfOld(chatRoom.Id);
                }
                else
                {
                    break;
                }
            }
        }
            
    }
}
