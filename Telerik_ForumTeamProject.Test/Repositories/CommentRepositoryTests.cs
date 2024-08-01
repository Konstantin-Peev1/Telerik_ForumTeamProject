using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Data;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Tests.Repositories
{
    [TestClass]
    public class CommentRepositoryTests
    {
        private DbContextOptions<ApplicationContext> _options;
        private ApplicationContext _context;
        private CommentRepository _commentRepository;

        [TestInitialize]
        public void Initialize()
        {
            var databaseName = $"TestDatabase_{Guid.NewGuid()}";
            _options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            _context = new ApplicationContext(_options);
            _commentRepository = new CommentRepository(_context);

            // Seed the database with initial data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var user = new User { ID = 1, UserName = "testuser1", FirstName = "Test", LastName = "User", Email = "test1@example.com", Password = "password1", Role = "User" };
            var post = new Post { Id = 1, Title = "Post 1", Content = "Content 1", Created = DateTime.Now, User = user };
            var comments = new List<Comment>
            {
                new Comment { Id = 1, Content = "Comment 1", PostID = 1, UserID = 1, Created = DateTime.Now },
                new Comment { Id = 2, Content = "Comment 2", PostID = 1, UserID = 1, Created = DateTime.Now, ParentCommentID = 1 }
            };

            _context.Users.Add(user);
            _context.Posts.Add(post);
            _context.Comments.AddRange(comments);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetAllPostComments_ShouldReturnAllComments_ForGivenPostId()
        {
            // Act
            var result = _commentRepository.GetAllPostComments(1);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Comment 1", result.First().Content);
        }

        [TestMethod]
        public void GetPagedReplies_ShouldReturnPagedReplies_ForGivenParentCommentId()
        {
            // Act
            var result = _commentRepository.GetPagedReplies(1, 1, 1);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Comment 2", result.First().Content);
        }

        [TestMethod]
        public void GetRepliesCount_ShouldReturnCorrectCount_ForGivenParentCommentId()
        {
            // Act
            var result = _commentRepository.GetRepliesCount(1);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void GetCommentById_ShouldReturnComment_WhenIdIsValid()
        {
            // Act
            var result = _commentRepository.GetCommentById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Comment 1", result.Content);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void GetCommentById_ShouldThrowException_WhenIdIsInvalid()
        {
            // Act
            var result = _commentRepository.GetCommentById(999);
        }

        [TestMethod]
        public void CreateComment_ShouldAddComment()
        {
            // Arrange
            var newComment = new Comment { Content = "New Comment", PostID = 1, UserID = 1, Created = DateTime.Now };

            // Act
            var createdComment = _commentRepository.CreateComment(newComment);

            // Assert
            Assert.IsNotNull(createdComment);
            Assert.AreEqual("New Comment", createdComment.Content);
            Assert.AreEqual(3, _context.Comments.Count());
        }

        [TestMethod]
        public void CreateReply_ShouldAddReply_ToExistingComment()
        {
            // Arrange
            var newReply = new Comment { Content = "New Reply", UserID = 1, Created = DateTime.Now };

            // Act
            var createdReply = _commentRepository.CreateReply(newReply, 1);

            // Assert
            Assert.IsNotNull(createdReply);
            Assert.AreEqual("New Reply", createdReply.Content);
            Assert.AreEqual(1, createdReply.ParentCommentID);
            Assert.AreEqual(3, _context.Comments.Count());
        }

        [TestMethod]
        public void UpdateComment_ShouldUpdateComment()
        {
            // Arrange
            var commentToUpdate = _commentRepository.GetCommentById(1);
            var updatedComment = new Comment { Content = "Updated Comment" };

            // Act
            var result = _commentRepository.UpdateComment(1, updatedComment);

            // Assert
            Assert.AreEqual("Updated Comment", result.Content);
        }

        [TestMethod]
        public void DeleteComment_ShouldRemoveComment()
        {
            // Arrange
            var commentToDelete = _commentRepository.GetCommentById(1);

            // Act
            var result = _commentRepository.DeleteComment(1, commentToDelete);

            // Assert
            Assert.IsTrue(result);
            
        }
    }
}
