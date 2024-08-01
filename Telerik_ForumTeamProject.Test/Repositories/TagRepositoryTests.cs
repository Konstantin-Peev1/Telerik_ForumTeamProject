using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories;

namespace Telerik_ForumTeamProject.Tests.Repositories
{
    [TestClass]
    public class TagRepositoryTests
    {
        private ApplicationContext _context;
        private TagRepository _tagRepository;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationContext(options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            SeedDatabase();

            _tagRepository = new TagRepository(_context);
        }

        private void SeedDatabase()
        {
            if (!_context.Users.Any(u => u.ID == 1))
            {
                var user = new User
                {
                    ID = 1,
                    UserName = "testuser",
                    Email = "test@example.com",
                    Password = "password",
                    Role = "User"
                };
                _context.Users.Add(user);
            }

            if (!_context.Posts.Any(p => p.Id == 1))
            {
                var post = new Post
                {
                    Id = 1,
                    Title = "Test Post",
                    Content = "This is a test post.",
                    UserID = 1,
                    Created = DateTime.Now,
                    Tags = new List<Tag> { }
                };
                _context.Posts.Add(post);
            }

            if (!_context.Tags.Any(t => t.ID == 1))
            {
                var tag = new Tag
                {
                    ID = 1,
                    Description = "TestTag"
                };
                _context.Tags.Add(tag);
            }

            _context.SaveChanges();
        }

        [TestMethod]
        public void Create_ShouldAddTag()
        {
            // Arrange
            var tag = new Tag
            {
                Description = "NewTag"
            };

            // Act
            var result = _tagRepository.Create(tag);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(tag.Description, result.Description);
            Assert.IsTrue(_context.Tags.Any(t => t.Description == "NewTag"));
        }

        [TestMethod]
        public void TagExists_ShouldReturnTrue_WhenTagExists()
        {
            // Act
            var result = _tagRepository.TagExists("TestTag");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TagExists_ShouldReturnFalse_WhenTagDoesNotExist()
        {
            // Act
            var result = _tagRepository.TagExists("NonExistentTag");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TagExistsFullDesc_ShouldReturnTag_WhenTagExists()
        {
            // Act
            var result = _tagRepository.TagExistsFullDesc("TestTag");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TestTag", result.Description);
        }

        [TestMethod]
        [ExpectedException(typeof(Telerik_ForumTeamProject.Exceptions.EntityNotFoundException))]
        public void TagExistsFullDesc_ShouldThrowException_WhenTagDoesNotExist()
        {
            // Act
            _tagRepository.TagExistsFullDesc("NonExistentTag");
        }

        [TestMethod]
        public void UpdateTags_ShouldAddTagToPost()
        {
            // Arrange
            var post = _context.Posts.First();
            var desc = "TestTag";
            post.Tags = new List<Tag>();
            // Act
            var result = _tagRepository.UpdateTags(post, desc);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(desc, result.Description);
            Assert.IsTrue(post.Tags.Any(t => t.Description == desc));
        }

        [TestMethod]
        public void RemoveTags_ShouldRemoveTagFromPost()
        {
            // Arrange
            var post = _context.Posts.First();
            post.Tags = new List<Tag>();
            var tag = _context.Tags.First();
            post.Tags.Add(tag);
            _context.SaveChanges();

            // Act
            var result = _tagRepository.RemoveTags(post, tag);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(post.Tags.Contains(tag));
        }

        [TestMethod]
        public void GetTagByDesc_ShouldReturnTags_WhenTagsExist()
        {
            // Act
            var result = _tagRepository.GetTagByDesc("TestTag");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("TestTag", result.First().Description);
        }

        [TestMethod]
        public void GetTagByDesc_ShouldReturnEmptyList_WhenTagsDoNotExist()
        {
            // Act
            var result = _tagRepository.GetTagByDesc("NonExistentTag");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
