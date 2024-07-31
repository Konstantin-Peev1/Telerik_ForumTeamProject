using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.ChatServiceTests
{
    [TestClass]
    public class GetActiveChats_Should
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
        public void ReturnActiveChats_When_TheyExist()
        {
            var result = this._sut.GetActiveChats();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}
