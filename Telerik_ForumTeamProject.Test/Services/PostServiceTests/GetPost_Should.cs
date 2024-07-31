using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.PostServiceTests
{
    [TestClass]
    public class GetPost_Should
    {
        private Mock<IPostRepository> _mockRepository;
        private PostService _sut;

        [TestInitialize]
        public void Setup()
        {
            var mockExamples = new MockPostRepository();
            this._mockRepository = mockExamples.GetMockRepository();
            this._sut = new PostService(this._mockRepository.Object);
        }

        [TestMethod]
        public void ReturnPost_When_ValidId()
        {
            var result = this._sut.GetPost(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("First Post", result.Title);
        }

        [TestMethod]
        public void ThrowException_When_PostNotFound()
        {
            Assert.ThrowsException<EntityNotFoundException>(() => this._sut.GetPost(999));
        }
    }
}
