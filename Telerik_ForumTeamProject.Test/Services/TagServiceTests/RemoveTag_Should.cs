using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.TagServiceTests
{
    [TestClass]
    public class RemoveTags_Should
    {
        private Mock<ITagRepository> _mockRepository;
        private TagService _sut;

        [TestInitialize]
        public void Setup()
        {
            var mockExamples = new MockTagRepository();
            this._mockRepository = mockExamples.GetMockRepository();
            this._sut = new TagService(this._mockRepository.Object);
        }

        [TestMethod]
        public void RemoveTags_When_ValidUser()
        {
            var user = new User
            {
                ID = 1,
                UserName = "Kosio",
                IsAdmin = false
            };

            var post = new Post
            {
                Id = 1,
                Title = "First Post",
                UserID = user.ID,
                Tags = new List<Tag> { this._mockRepository.Object.GetTagByDesc("Tag1").FirstOrDefault() }
            };

            var result = this._sut.RemoveTags(user, post, "Tag1");

            Assert.IsTrue(result);
            Assert.AreEqual(0, post.Tags.Count);
        }

        [TestMethod]
        public void ThrowException_When_RemovingOtherUsersTags()
        {
            var otherUser = new User
            {
                ID = 2,
                UserName = "Kosio1",
                IsAdmin = false
            };

            var post = new Post
            {
                Id = 1,
                Title = "First Post",
                UserID = 1,
                Tags = new List<Tag> { new Tag { Description = "Tag1" } }
            };

            Assert.ThrowsException<AuthorisationExcpetion>(() => this._sut.RemoveTags(otherUser, post, "Tag1"));
        }
    }
}
