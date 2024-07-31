using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.CommentServiceTests
{
    [TestClass]
    public class GetPagedReplies_Should
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
        public void ReturnPagedReplies_When_ValidParentCommentId()
        {
            var comment = _mockRepository.Object.GetCommentById(2);
           // var parentCommentId = 2;
            var page = 1;
            var pageSize = 1;
            User user = new User
            {
                ID = 1,
                UserName = "User1",
                Password = "password1",
                FirstName = "FirstName1",
                LastName = "LastName1",
                Email = "user1@example.com",
                IsAdmin = false,
                IsBlocked = false
            };
            Comment comment1 = new Comment
            {
                Id = 1,
                Content = "First comment",
                UserID = 1,
                PostID = 1,
                Created = DateTime.UtcNow.AddDays(-1),
                Replies = new List<Comment>()
            };
            var addComment = this._sut.CreateReply(comment1, 2, user);
            var result = this._sut.GetPagedReplies(comment.Id, page, pageSize);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(1, result.Metadata.TotalPages);
        }

        [TestMethod]
        public void ThrowException_When_PageNotFound()
        {
            var parentCommentId = 2;
            var page = 2;
            var pageSize = 1;

            Assert.ThrowsException<PageNotFoundException>(() => this._sut.GetPagedReplies(parentCommentId, page, pageSize));
        }
    }
}
