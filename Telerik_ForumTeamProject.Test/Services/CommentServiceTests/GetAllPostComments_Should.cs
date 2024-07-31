using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.CommentServiceTests
{
    [TestClass]
    public class GetAllPostComments_Should
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
        public void ReturnAllPostComments_When_ValidPostId()
        {
            var postId = 1;

            var result = this._sut.GetAllPostComments(postId);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void ReturnEmptyList_When_InvalidPostId()
        {
            var postId = 999;

            var result = this._sut.GetAllPostComments(postId);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
