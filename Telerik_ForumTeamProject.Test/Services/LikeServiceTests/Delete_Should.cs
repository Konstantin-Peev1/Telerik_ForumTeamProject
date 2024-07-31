using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Test.Services.LikeServiceTests
{
    [TestClass]
    public class Delete_Should
    {
        private Mock<ILikeRepository> _mockRepository;
        private Mock<IPostService> _mockPostService;
        private LikeService _sut;

        [TestInitialize]
        public void Setup()
        {
            var mockExamples = new MockLikeRepository();
            this._mockRepository = mockExamples.GetMockRepository();
            this._mockPostService = new Mock<IPostService>();

            this._sut = new LikeService(this._mockRepository.Object, this._mockPostService.Object);
        }

        [TestMethod]
        public void RemoveLike_When_ValidLike()
        {
            var post = new Post
            {
                Id = 1,
                Title = "First Post",
                Likes = new List<Like>
                {
                    new Like
                    {
                        Id = 1,
                        UserId = 1,
                        PostID = 1
                    }
                }
            };
            var user = new User
            {
                ID = 1,
                UserName = "User1"
            };
            var like = post.Likes.First();

            _mockPostService.Setup(x => x.GetPost(It.IsAny<int>())).Returns(post);

            var result = this._sut.Delete(post.Id, user, like);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.ID, result.UserId);
            Assert.AreEqual(post.Id, result.PostID);
            Assert.AreEqual(0, post.Likes.Count);
        }
    }
}
