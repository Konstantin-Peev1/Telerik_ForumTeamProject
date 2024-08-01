using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;

namespace Telerik_ForumTeamProject.Tests.ModelMapperTests
{
    [TestClass]
    public class ModelMapperTests
    {
        private ModelMapper _mapper;

        [TestInitialize]
        public void SetUp()
        {
            _mapper = new ModelMapper();
        }

        [TestMethod]
        public void MapUser_ShouldReturnUserResponseDTO_When_UserIsValid()
        {
            // Arrange
            var user = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Posts = new List<Post> { new Post { Title = "Post1" } }
            };

            // Act
            var result = _mapper.Map(user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.FirstName, result.FirstName);
            Assert.AreEqual(user.LastName, result.LastName);
            Assert.AreEqual(user.Email, result.Email);
            Assert.AreEqual(user.Posts.Count, result.Posts.Count);
        }

        [TestMethod]
        public void MapUserRequest_ShouldReturnUser_When_UserRequestDTOIsValid()
        {
            // Arrange
            var userRequest = new UserRequestDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password",
                UserName = "john.doe"
            };

            // Act
            var result = _mapper.Map(userRequest);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userRequest.FirstName, result.FirstName);
            Assert.AreEqual(userRequest.LastName, result.LastName);
            Assert.AreEqual(userRequest.Email, result.Email);
            Assert.AreEqual(userRequest.Password, result.Password);
            Assert.AreEqual(userRequest.UserName, result.UserName);
        }

        [TestMethod]
        public void MapLike_ShouldReturnLikeResponseDTO_When_LikeIsValid()
        {
            // Arrange
            var like = new Like
            {
                User = new User { UserName = "john.doe" }
            };

            // Act
            var result = _mapper.Map(like);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(like.User.UserName, result.UserName);
        }

        [TestMethod]
        public void MapPostRequest_ShouldReturnPost_When_PostRequestDTOIsValid()
        {
            // Arrange
            var postRequestDTO = new PostRequestDTO
            {
                Title = "Post Title",
                Content = "Post Content"
            };

            // Act
            var result = _mapper.Map(postRequestDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(postRequestDTO.Title, result.Title);
            Assert.AreEqual(postRequestDTO.Content, result.Content);
        }

        [TestMethod]
        public void MapPost_ShouldReturnPostUploadResponseDTO_When_PostIsValid()
        {
            // Arrange
            var post = new Post
            {
                Title = "Post Title",
                Content = "Post Content",
                Created = DateTime.Now,
                User = new User { UserName = "john.doe" },
                Likes = new List<Like>(),
                Comments = new List<Comment>(),
                Tags = new List<Tag> { new Tag { Description = "Tag1" } }
            };

            // Act
            var result = _mapper.Map(post);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(post.Title, result.Title);
            Assert.AreEqual(post.Content, result.Content);
            Assert.AreEqual(post.User.UserName, result.UserName);
            Assert.AreEqual(post.Tags.Count, result.Tags.Count);
            Assert.AreEqual(post.Tags.First().Description, result.Tags.First());
        }

        [TestMethod]
        public void MapTag_ShouldReturnTagResponseDTO_When_TagIsValid()
        {
            // Arrange
            var tag = new Tag
            {
                Description = "Tag Description",
                Posts = new List<Post> { new Post { Title = "Post1", User = new User { UserName = "John" } } }
            };

            // Act
            var result = _mapper.Map(tag);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(tag.Description, result.Description);
            Assert.AreEqual(tag.Posts.Count, result.Posts.Count);
        }

        [TestMethod]
        public void MapComment_ShouldReturnCommentReplyResponseDTO_When_CommentIsValid()
        {
            // Arrange
            var comment = new Comment
            {
                Content = "Comment Content",
                Created = DateTime.Now,
                User = new User { UserName = "john.doe" }
            };

            // Act
            var result = _mapper.Map(comment);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(comment.Content, result.Content);
            Assert.AreEqual(comment.User.UserName, result.UserName);
        }

        [TestMethod]
        public void MapComments_ShouldReturnCommentReplyResponseDTOList_When_CommentsAreValid()
        {
            // Arrange
            var comments = new List<Comment>
            {
                new Comment { Content = "Comment1", Created = DateTime.Now, User = new User { UserName = "john.doe" } },
                new Comment { Content = "Comment2", Created = DateTime.Now, User = new User { UserName = "jane.doe" } }
            };

            // Act
            var result = _mapper.Map(comments);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(comments.Count, result.Count);
            Assert.AreEqual(comments.First().Content, result.First().Content);
        }

        [TestMethod]
        public void MapReplyResponse_ShouldReturnReplyResponseDTOList_When_RepliesAreValid()
        {
            // Arrange
            var replies = new List<Comment>
            {
                new Comment { Content = "Reply1", Created = DateTime.Now, User = new User { UserName = "john.doe" } },
                new Comment { Content = "Reply2", Created = DateTime.Now, User = new User { UserName = "jane.doe" } }
            };

            // Act
            var result = _mapper.MapReplyResponse(replies);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(replies.Count, result.Count);
            Assert.AreEqual(replies.First().Content, result.First().Content);
        }

        [TestMethod]
        public void MapCommentRequest_ShouldReturnComment_When_CommentRequestDTOIsValid()
        {
            // Arrange
            var commentRequest = new CommentRequestDTO { Content = "Comment Content" };

            // Act
            var result = _mapper.Map(commentRequest, 1, 0);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(commentRequest.Content, result.Content);
            Assert.AreEqual(1, result.PostID);
        }

        [TestMethod]
        public void MapUpdatedCommentRequest_ShouldReturnUpdatedComment_When_CommentRequestDTOIsValid()
        {
            // Arrange
            var commentRequest = new CommentRequestDTO { Content = "Updated Comment" };

            // Act
            var result = _mapper.Map(commentRequest, 0, 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(commentRequest.Content, result.Content);
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void MapPosts_ShouldReturnPostResponseDTOList_When_PostsAreValid()
        {
            // Arrange
            var posts = new List<Post>
            {
                new Post { Title = "Post1" },
                new Post { Title = "Post2" }
            };

            // Act
            var result = _mapper.Map(posts);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(posts.Count, result.Count);
            Assert.AreEqual(posts.First().Title, result.First().Title);
        }
    }
}
