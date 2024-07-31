using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.CommentServiceTests
{
    [TestClass]
    public class CreateComment_Should
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
        public void CreateComment_When_Valid()
        {
            var comment = new Comment
            {
                Content = "New comment",
                PostID = 1
            };
            var user = new User
            {
                ID = 1,
                UserName = "User1"
            };

            var result = this._sut.CreateComment(comment, user);

            Assert.IsNotNull(result);
            Assert.AreEqual(comment.Content, result.Content);
            Assert.AreEqual(user.ID, result.UserID);
        }
    }
}
