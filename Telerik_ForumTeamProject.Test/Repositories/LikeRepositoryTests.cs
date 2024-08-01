using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories;

namespace Telerik_ForumTeamProject.Tests.Repositories
{
    [TestClass]
    public class LikeRepositoryTests
    {
        private ApplicationContext _context;
        private LikeRepository _likeRepository;

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

            _likeRepository = new LikeRepository(_context);
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
                    , FirstName = "Test",
                    LastName = "Test",
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
                    Created = DateTime.Now
                };
                _context.Posts.Add(post);
            }
            var userEntity = _context.Users.First(u => u.ID == 1);
            var postEntity = _context.Posts.First(p => p.Id == 1);

            if (!_context.Set<Like>().Any(l => l.UserId == 1 && l.PostID == 1))
            {
                var like = new Like
                {
                    UserId = userEntity.ID,
                    PostID = postEntity.Id,
                    User = userEntity,
                    Post = postEntity
                };

                userEntity.Likes = new List<Like> { like };
                postEntity.Likes = new List<Like> { like };

                _context.SaveChanges();
            }
        }

        [TestMethod]
        public void Create_ShouldAddLike()
        {
            // Arrange
            var user = _context.Users.First();
            var post = _context.Posts.First();
            var like = new Like
            {
                UserId = user.ID,
                PostID = post.Id,
                User = user,
                Post = post
            };

            // Act
            var result = _likeRepository.Create(user, post, like);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, post.Likes.Count);
            Assert.AreEqual(2, user.Likes.Count);
        }

        [TestMethod]
        public void Remove_ShouldDeleteLike()
        {
            // Arrange
            var user = _context.Users.First();
            var post = _context.Posts.First();
            var like = new Like
            {
                UserId = user.ID,
                PostID = post.Id,
                User = user,
                Post = post
            };

            

            // Act
            var result = _likeRepository.Remove(user, post, like);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, post.Likes.Count);
            Assert.AreEqual(1, user.Likes.Count);
        }
    }
}
