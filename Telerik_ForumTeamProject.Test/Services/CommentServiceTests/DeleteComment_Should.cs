using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.CommentServiceTests
{
    [TestClass]
    public class DeleteComment_Should
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
        public void DeleteComment_When_Valid()
        {
            var commentId = 1;
            var user = new User
            {
                ID = 1,
                UserName = "User1",
                IsAdmin = false
            };

            var result = this._sut.DeleteComment(commentId, user);

            Assert.IsTrue(result);
            Assert.ThrowsException<EntityNotFoundException>(() => this._sut.GetComment(commentId));
        }

        [TestMethod]
        public void ThrowException_When_NotAuthorized()
        {
            var commentId = 1;
            var user = new User
            {
                ID = 2,
                UserName = "User2",
                IsAdmin = false
            };

            Assert.ThrowsException<AuthorisationExcpetion>(() => this._sut.DeleteComment(commentId, user));
        }
    }
}
