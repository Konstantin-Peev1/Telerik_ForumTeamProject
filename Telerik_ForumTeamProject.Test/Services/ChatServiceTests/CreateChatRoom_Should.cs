using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.ChatServiceTests
{
    [TestClass]
    public class CreateChatRoom_Should
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
        public void CreateChatRoom_When_Valid()
        {
            var user = new User
            {
                ID = 1,
                UserName = "User1"
            };
            var chatRoomName = "New Chat Room";

            _sut.CreateChatRoom(chatRoomName, user);

            var chatRooms = _mockRepository.Object.GetActiveChats();
            var newChatRoom = chatRooms.FirstOrDefault(cr => cr.Name == chatRoomName);

            Assert.IsNotNull(_mockRepository.Object.GetChatRoom(3));
         
        }
    }
}
