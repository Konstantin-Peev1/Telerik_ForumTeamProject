using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.ChatServiceTests
{
    [TestClass]
    public class AddMessage_Should
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
        public void AddMessage_When_Valid()
        {
            var chatRoomId = 1;
            var userId = 1;
            var userName = "User1";
            var message = "New message";

            _sut.AddMessage(chatRoomId, userId, userName, message);

            var chatRoom = _mockRepository.Object.GetChatRoom(chatRoomId);
            Assert.IsNotNull(chatRoom);
            Assert.AreEqual(2, chatRoom.Messages.Count);
            Assert.AreEqual(message, chatRoom.Messages.Last().Message);
        }
    }
}
