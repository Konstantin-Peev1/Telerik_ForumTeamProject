using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Test
{
    public class MockChatRepository
    {
        private List<ChatRoom> sampleChatRooms;
        private List<ChatMessage> sampleChatMessages;
        private List<User> sampleUsers;

        public Mock<IChatRepository> GetMockRepository()
        {
            var mockRepository = new Mock<IChatRepository>();

            sampleUsers = new List<User>
            {
                new User
                {
                    ID = 1,
                    UserName = "User1",
                    Password = "password1",
                    FirstName = "FirstName1",
                    LastName = "LastName1",
                    Email = "user1@example.com",
                    IsAdmin = false,
                    IsBlocked = false
                },
                new User
                {
                    ID = 2,
                    UserName = "User2",
                    Password = "password2",
                    FirstName = "FirstName2",
                    LastName = "LastName2",
                    Email = "user2@example.com",
                    IsAdmin = true,
                    IsBlocked = false
                }
            };

            sampleChatRooms = new List<ChatRoom>
            {
                new ChatRoom
                {
                    Id = 1,
                    Name = "Chat Room 1",
                    UserId = 1,
                    Creator = sampleUsers[0],
                    Created = DateTime.UtcNow.AddHours(-21),
                    Messages = new List<ChatMessage>()
                },
                new ChatRoom
                {
                    Id = 2,
                    Name = "Chat Room 2",
                    UserId = 2,
                    Creator = sampleUsers[1],
                    Created = DateTime.UtcNow.AddHours(-12),
                    Messages = new List<ChatMessage>()
                }
            };

            sampleChatMessages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Id = 1,
                    UserId = 1,
                    UserName = "User1",
                    Message = "Hello World!",
                    Created = DateTime.UtcNow.AddHours(-23),
                    ChatRoomId = 1,
                    ChatRoom = sampleChatRooms[0]
                },
                new ChatMessage
                {
                    Id = 2,
                    UserId = 2,
                    UserName = "User2",
                    Message = "Hi there!",
                    Created = DateTime.UtcNow.AddHours(-1),
                    ChatRoomId = 2,
                    ChatRoom = sampleChatRooms[1]
                }
            };

            sampleChatRooms[0].Messages.Add(sampleChatMessages[0]);
            sampleChatRooms[1].Messages.Add(sampleChatMessages[1]);

            // Setup for GetActiveChats
            mockRepository.Setup(x => x.GetActiveChats())
                .Returns(sampleChatRooms.Where(chatRoom => (DateTime.UtcNow - chatRoom.Created).TotalDays < 1).ToList());

            // Setup for GetChatRoom
            mockRepository.Setup(x => x.GetChatRoom(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    var chatRoom = sampleChatRooms.FirstOrDefault(chatRoom => chatRoom.Id == id);
                    if (chatRoom == null)
                    {
                        throw new EntityNotFoundException($"Chat room with ID {id} not found.");
                    }
                    return chatRoom;
                });

            // Setup for AddMessage
            mockRepository.Setup(x => x.AddMessage(It.IsAny<ChatMessage>()))
                .Callback((ChatMessage message) =>
                {
                    message.Id = sampleChatMessages.Max(m => m.Id) + 1;
                    sampleChatMessages.Add(message);
                    var chatRoom = sampleChatRooms.FirstOrDefault(c => c.Id == message.ChatRoomId);
                    if (chatRoom != null)
                    {
                        chatRoom.Messages.Add(message);
                    }
                });

            // Setup for AddChatRoom
            mockRepository.Setup(x => x.AddChatRoom(It.IsAny<ChatRoom>()))
                .Callback((ChatRoom chatRoom) =>
                {
                    chatRoom.Id = sampleChatRooms.Max(c => c.Id) + 1;
                    sampleChatRooms.Add(chatRoom);
                });

            // Setup for DeleteChatRoom
            mockRepository.Setup(x => x.DeleteChatRoom(It.IsAny<ChatRoom>()))
                .Callback((ChatRoom chatRoom) =>
                {
                    sampleChatRooms.Remove(chatRoom);
                    sampleChatMessages.RemoveAll(m => m.ChatRoomId == chatRoom.Id);
                });

            // Setup for DeleteChatRoomIfOld
            mockRepository.Setup(x => x.DeleteChatRoomIfOld(It.IsAny<int>()))
                .Callback((int chatRoomId) =>
                {
                    var chatRoom = sampleChatRooms.FirstOrDefault(c => c.Id == chatRoomId);
                    if (chatRoom != null && (DateTime.UtcNow - chatRoom.Created).TotalHours > 24)
                    {
                        sampleChatRooms.Remove(chatRoom);
                        sampleChatMessages.RemoveAll(m => m.ChatRoomId == chatRoomId);
                    }
                });

            return mockRepository;
        }
    }
}
