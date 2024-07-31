using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.PostServiceTests
{
    [TestClass]
    public class UpdatePost_Should
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
        public void UpdatePost_When_ValidUser()
        {
            var user = new User
            {
                ID = 1,
                UserName = "Kosio",
                IsAdmin = false
            };

            var updatedPost = new Post
            {
                Title = "Updated Post",
                Content = "Updated content of the post"
            };

            var result = this._sut.UpdatePost(1, updatedPost, user);

            Assert.IsNotNull(result);
            Assert.AreEqual(updatedPost.Title, result.Title);
            Assert.AreEqual(updatedPost.Content, result.Content);
        }

        [TestMethod]
        public void ThrowException_When_UpdatingOtherUsersPost()
        {
            var otherUser = new User
            {
                ID = 2,
                UserName = "Kosio1",
                IsAdmin = false
            };

            var updatedPost = new Post
            {
                Title = "Updated Post",
                Content = "Updated content of the post"
            };

            Assert.ThrowsException<AuthorisationExcpetion>(() => this._sut.UpdatePost(1, updatedPost, otherUser));
        }
    }
}
