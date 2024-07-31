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
    public class UpdateTags_Should
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
        public void UpdateTags_When_ValidUser()
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
                Tags = new List<Tag> { new Tag { Description = "Tag1" } }
            };

            var result = this._sut.UpdateTags(user, post, "Tag2");

            Assert.IsNotNull(result);
            Assert.AreEqual(2, post.Tags.Count);
            Assert.IsTrue(post.Tags.Any(t => t.Description == "Tag2"));
        }

        [TestMethod]
        public void ThrowException_When_UpdatingOtherUsersTags()
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

            Assert.ThrowsException<AuthorisationExcpetion>(() => this._sut.UpdateTags(otherUser, post, "Tag2"));
        }

        [TestMethod]
        public void ThrowException_When_TooManyTags()
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
                Tags = new List<Tag>
                {
                    new Tag { Description = "Tag1" },
                    new Tag { Description = "Tag2" }
                }
            };


            Assert.ThrowsException<AuthorisationExcpetion>(() => this._sut.UpdateTags(user, post, "Tag3"));
        }

        [TestMethod]
        public void ThrowException_When_TagAlreadyExists()
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
                Tags = new List<Tag> { new Tag { Description = "Tag1" } }
            };

            Assert.ThrowsException<DuplicateEntityException>(() => this._sut.UpdateTags(user, post, "Tag1"));
        }
    }
}
