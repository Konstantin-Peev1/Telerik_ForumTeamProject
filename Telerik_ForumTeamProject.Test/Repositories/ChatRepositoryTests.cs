using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories;

namespace Telerik_ForumTeamProject.Tests.Repositories
{
    [TestClass]
    public class ChatRepositoryTests
    {
        private ApplicationContext _context;
        private ChatRepository _repository;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Ensure unique database for each test
                .Options;

            _context = new ApplicationContext(options);
            _repository = new ChatRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var user = new User
            {
                ID = 1,
                UserName = "TestUser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "password",
                Role = "User"
            };

            var chatRoom = new ChatRoom
            {
                Id = 1,
                Name = "TestChatRoom",
                Created = DateTime.UtcNow.AddHours(-1),
                UserId = 1,
                Creator = user,
                Messages = new List<ChatMessage>()
            };

            _context.Database.EnsureDeleted(); // Clear the database to avoid duplicate keys
            _context.Users.Add(user);
            _context.ChatRooms.Add(chatRoom);
            _context.SaveChanges();
        }

        [TestMethod]
        public void AddMessage_ShouldAddMessageToChatRoom()
        {
            // Arrange
            var message = new ChatMessage
            {
                Id = 1,
                UserId = 1,
                UserName = "TestUser",
                Message = "Test Message",
                Created = DateTime.UtcNow,
                ChatRoomId = 1
            };

            // Act
            _repository.AddMessage(message);

            // Assert
            var chatRoom = _repository.GetChatRoom(1);
            Assert.AreEqual(1, chatRoom.Messages.Count);
            Assert.AreEqual("Test Message", chatRoom.Messages.First().Message);
        }

        [TestMethod]
        public void GetActiveChats_ShouldReturnAllChatRooms()
        {
            // Act
            var chatRooms = _repository.GetActiveChats();

            // Assert
            Assert.AreEqual(1, chatRooms.Count);
            Assert.AreEqual("TestChatRoom", chatRooms.First().Name);
        }

        [TestMethod]
        public void GetChatRoom_ShouldReturnChatRoomById()
        {
            // Act
            var chatRoom = _repository.GetChatRoom(1);

            // Assert
            Assert.IsNotNull(chatRoom);
            Assert.AreEqual("TestChatRoom", chatRoom.Name);
        }

        [TestMethod]
        public void AddChatRoom_ShouldAddNewChatRoom()
        {
            // Arrange
            var newChatRoom = new ChatRoom
            {
                Id = 2,
                Name = "NewChatRoom",
                Created = DateTime.UtcNow,
                UserId = 1,
                Creator = _context.Users.First()
            };

            // Act
            _repository.AddChatRoom(newChatRoom);

            // Assert
            var chatRooms = _repository.GetActiveChats();
            Assert.AreEqual(2, chatRooms.Count);
            Assert.IsTrue(chatRooms.Any(cr => cr.Name == "NewChatRoom"));
        }

        [TestMethod]
        public void DeleteChatRoom_ShouldRemoveChatRoom()
        {
            // Act
            var chatRoom = _repository.GetChatRoom(1);
            _repository.DeleteChatRoom(chatRoom);

            // Assert
            var chatRooms = _repository.GetActiveChats();
            Assert.AreEqual(0, chatRooms.Count);
        }

        [TestMethod]
        public void DeleteChatRoomIfOld_ShouldRemoveOldChatRoom()
        {
            // Arrange
            var oldChatRoom = new ChatRoom
            {
                Id = 2,
                Name = "OldChatRoom",
                Created = DateTime.UtcNow.AddDays(-2),
                UserId = 1,
                Creator = _context.Users.First()
            };
            _repository.AddChatRoom(oldChatRoom);

            // Act
            _repository.DeleteChatRoomIfOld(2);

            // Assert
            var chatRooms = _repository.GetActiveChats();
            Assert.AreEqual(1, chatRooms.Count);
            Assert.IsFalse(chatRooms.Any(cr => cr.Name == "OldChatRoom"));
        }
    }
}
