using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.ChatServiceTests
{
    [TestClass]
    public class GetChatRoom_Should
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
        public void ReturnChatRoom_When_ValidId()
        {
            var result = this._sut.GetChatRoom(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Chat Room 1", result.Name);
        }

        [TestMethod]
        public void ThrowException_When_ChatRoomNotFound()
        {
            Assert.ThrowsException<EntityNotFoundException>(() => this._sut.GetChatRoom(999));
        }
    }
}
