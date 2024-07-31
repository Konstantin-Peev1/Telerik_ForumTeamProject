using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.CommentServiceTests
{
    [TestClass]
    public class GetComment_Should
    {
        private Mock<ICommentRepository> _mockRepository;
        private CommentService _sut;

        [TestInitialize]
        public void Setup()
        {
            var mockExamples = new MockCommentRepository();
            this._mockRepository = mockExamples.GetMockRepository();
            this._sut = new CommentService(this._mockRepository.Object);
        }

        [TestMethod]
        public void ReturnComment_When_ValidId()
        {
            var commentId = 1;

            var result = this._sut.GetComment(commentId);

            Assert.IsNotNull(result);
            Assert.AreEqual("First comment", result.Content);
        }

        [TestMethod]
        public void ThrowException_When_CommentNotFound()
        {
            var commentId = 999;

            Assert.ThrowsException<EntityNotFoundException>(() => this._sut.GetComment(commentId));
        }
    }
}
