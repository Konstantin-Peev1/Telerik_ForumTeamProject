using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.PostServiceTests
{
    [TestClass]
    public class CreatePost_Should
    {
        private Mock<IPostRepository> _mockRepository;
        private PostService _sut;
        private IConfiguration _configuration;

        [TestInitialize]
        public void Setup()
        {
            var mockExamples = new MockPostRepository();
            this._mockRepository = mockExamples.GetMockRepository();

            var inMemorySettings = new Dictionary<string, string> {
                {"CloudinarySettings:DefaultProfilePictureUrl", "http://example.com/default-profile-picture.png"},
            };

            this._configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            this._sut = new PostService(this._mockRepository.Object);
        }

        [TestMethod]
        public void CreatePost_When_ValidUser()
        {
            var newUser = new User
            {
                ID = 4,
                UserName = "NewUser",
                IsBlocked = false
            };

            var newPost = new Post
            {
                Title = "New Post",
                Content = "Content of the new post",
                UserID = newUser.ID,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            var createdPost = this._sut.CreatePost(newPost, newUser);

            Assert.IsNotNull(createdPost);
            Assert.AreEqual(newPost.Title, createdPost.Title);
            Assert.AreEqual(newPost.Content, createdPost.Content);
            Assert.AreEqual(newUser.ID, createdPost.UserID);
        }

        [TestMethod]
        public void ThrowException_When_BlockedUser()
        {
            var blockedUser = new User
            {
                ID = 3,
                UserName = "BlockedUser",
                IsBlocked = true
            };

            var newPost = new Post
            {
                Title = "New Post",
                Content = "Content of the new post",
                UserID = blockedUser.ID,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            Assert.ThrowsException<AuthorisationExcpetion>(() => this._sut.CreatePost(newPost, blockedUser));
        }
    }
}
