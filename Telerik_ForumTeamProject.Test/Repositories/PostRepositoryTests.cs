using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Repositories;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Tests.Repositories
{
    [TestClass]
    public class PostRepositoryTests
    {
        private DbContextOptions<ApplicationContext> _options;
        private ApplicationContext _context;
        private PostRepository _postRepository;

        [TestInitialize]
        public void Initialize()
        {
            var databaseName = $"TestDatabase_{Guid.NewGuid()}";
            _options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            _context = new ApplicationContext(_options);
            _postRepository = new PostRepository(_context);

            // Seed the database with initial data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var user = new User { ID = 1, UserName = "testuser1", FirstName = "Test", LastName = "User", Email = "test1@example.com", Password = "password1", Role = "User" };
            var posts = new List<Post>
            {
                new Post { Id = 1, Title = "Post 1", Content = "Content 1", Created = DateTime.Now, User = user, Likes = new List<Like>(), Comments = new List<Comment>(), Tags = new List<Tag>() },
                new Post { Id = 2, Title = "Post 2", Content = "Content 2", Created = DateTime.Now.AddDays(-1), User = user, Likes = new List<Like>(), Comments = new List<Comment>(), Tags = new List<Tag>() }
            };

            _context.Users.Add(user);
            _context.Posts.AddRange(posts);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetTop10Commented_ShouldReturnTop10CommentedPosts()
        {
            // Act
            var result = _postRepository.GetTop10Commented();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Post 1", result.First().Title);
        }

        [TestMethod]
        public void GetTop10Recent_ShouldReturnTop10RecentPosts()
        {
            // Act
            var result = _postRepository.GetTop10Recent();

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Post 1", result.First().Title);
        }

        [TestMethod]
        public void GetPost_ShouldReturnPost_WhenIdIsValid()
        {
            // Act
            var result = _postRepository.GetPost(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void GetPost_ShouldThrowException_WhenIdIsInvalid()
        {
            // Act
            var result = _postRepository.GetPost(999);
        }

        [TestMethod]
        public void CreatePost_ShouldAddPost()
        {
            // Arrange
            var newPost = new Post { Title = "New Post", Content = "New Content", Created = DateTime.Now, User = _context.Users.First() };

            // Act
            var createdPost = _postRepository.CreatePost(newPost);

            // Assert
            Assert.IsNotNull(createdPost);
            Assert.AreEqual("New Post", createdPost.Title);
            Assert.AreEqual(3, _context.Posts.Count());
        }

        [TestMethod]
        public void FilterBy_ShouldReturnFilteredPosts()
        {
            // Arrange
            var filterParams = new PostQueryParamteres { Title = "Post 1" };

            // Act
            var result = _postRepository.FilterBy(filterParams);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Post 1", result.First().Title);
        }

        [TestMethod]
        public void FilterBy_WithPagination_ShouldReturnPagedPosts()
        {
            // Arrange
            var filterParams = new PostQueryParamteres { Title = "Post" };

            // Act
            var result = _postRepository.FilterBy(1, 1, filterParams);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Post 1", result.First().Title);
        }

        [TestMethod]
        public void UpdatePost_ShouldUpdatePost()
        {
            // Arrange
            var postToUpdate = _postRepository.GetPost(1);
            var updatedPost = new Post { Title = "Updated Post", Content = "Updated Content" };

            // Act
            var result = _postRepository.UpdatePost(postToUpdate, updatedPost);

            // Assert
            Assert.AreEqual("Updated Post", result.Title);
            Assert.AreEqual("Updated Content", result.Content);
        }

        [TestMethod]
        public void GetPostsCount_ShouldReturnCorrectCount()
        {
            // Act
            var result = _postRepository.GetPostsCount();

            // Assert
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void DeletePost_ShouldRemovePost()
        {
            // Arrange
            var postToDelete = _postRepository.GetPost(1);

            // Act
            var result = _postRepository.DeletePost(postToDelete);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(1, _context.Posts.Count());
        }
    }
}
