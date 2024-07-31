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
    public class DeletePost_Should
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
        public void DeletePost_When_ValidUser()
        {
            var postOwner = new User
            {
                ID = 1,
                UserName = "Kosio",
                IsAdmin = false
            };

            var result = this._sut.DeletePost(1, postOwner);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ThrowException_When_DeletingOtherUsersPost()
        {
            var otherUser = new User
            {
                ID = 2,
                UserName = "Kosio1",
                IsAdmin = false
            };

            Assert.ThrowsException<AuthorisationExcpetion>(() => this._sut.DeletePost(1, otherUser));
        }

        [TestMethod]
        public void DeletePost_When_AdminUser()
        {
            var adminUser = new User
            {
                ID = 3,
                UserName = "Admin",
                IsAdmin = true
            };

            var result = this._sut.DeletePost(1, adminUser);

            Assert.IsTrue(result);
        }
    }
}
