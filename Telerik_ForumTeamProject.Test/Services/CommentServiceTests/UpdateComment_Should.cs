using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.CommentServiceTests
{
    [TestClass]
    public class UpdateComment_Should
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
        public void UpdateComment_When_Valid()
        {
            var commentId = 1;
            var updatedContent = "Updated content";
            var comment = new Comment
            {
                Content = updatedContent
            };
            var user = new User
            {
                ID = 1,
                UserName = "User1",
                IsAdmin = true,
            };

            var result = this._sut.UpdateComment(commentId, comment, user);

            Assert.IsNotNull(result);
            Assert.AreEqual(updatedContent, result.Content);
        }

        [TestMethod]
        public void ThrowException_When_NotAuthorized()
        {
            var commentId = 1;
            var updatedContent = "Updated content";
            var comment = new Comment
            {
                Content = updatedContent
            };
            var user = new User
            {
                ID = 2,
                UserName = "User2",
                IsAdmin = false
            };

            Assert.ThrowsException<AuthorisationExcpetion>(() => this._sut.UpdateComment(commentId, comment, user));
        }
    }
}
