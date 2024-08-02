using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.ChatServiceTests
{
    [TestClass]
    public class DeleteChatRoom_Should
    {
        private Mock<IChatRepository> _mockRepository;
        private ChatService _sut;

        [TestInitialize]
        public void Setup()
        {
            var mockExamples = new MockChatRepository();
            this._mockRepository = mockExamples.GetMockRepository();
            this._sut = new ChatService(this._mockRepository.Object);
        }

        [TestMethod]
        public void DeleteChatRoom_When_Owner_And_OlderThan24Hours()
        {
            var user = new User
            {
                ID = 1,
                UserName = "User1",
                IsAdmin = false
            };
            var chatRoomId = 1;

            var chatRoom = _mockRepository.Object.GetChatRoom(chatRoomId);
            chatRoom.Created = DateTime.UtcNow.AddDays(-2); // Older than 24 hours

            _sut.DeleteChatRoomIFOld();
            


            Assert.ThrowsException<EntityNotFoundException>(() => _mockRepository.Object.GetChatRoom(chatRoomId)); ;
        }

        [TestMethod]
        public void DeleteChatRoom_When_Owner_And_LessThan24Hours()
        {
            var user = new User
            {
                ID = 1,
                UserName = "User1",
                IsAdmin = false
            };
            var chatRoomId = 1;

            
           

            _sut.DeleteChatRoom(chatRoomId, user);


            Assert.ThrowsException<EntityNotFoundException>(() => _mockRepository.Object.GetChatRoom(chatRoomId)); ;
        }

        [TestMethod]
        public void DeleteChatRoom_When_Admin()
        {
            var user = new User
            {
                ID = 2,
                UserName = "AdminUser",
                IsAdmin = true
            };
            var chatRoomId = 1;


            _sut.DeleteChatRoom(chatRoomId, user);

            Assert.ThrowsException<EntityNotFoundException>(() => _mockRepository.Object.GetChatRoom(chatRoomId));
        }

        [TestMethod]
        public void ThrowException_When_NotOwnerOrAdmin()
        {
            var user = new User
            {
                ID = 3,
                UserName = "User3",
                IsAdmin = false
            };
            var chatRoomId = 1;

            Assert.ThrowsException<UnauthorizedAccessException>(() => _sut.DeleteChatRoom(chatRoomId, user));
        }
    }
}
