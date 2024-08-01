using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Controllers;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Tests.Controllers.CommentControllerAPITests
{
    [TestClass]
    public class CommentControllerAPITests
    {
        private Mock<ICommentService> _commentServiceMock;
        private Mock<IPostService> _postServiceMock;
        private Mock<ModelMapper> _modelMapperMock;
        private Mock<AuthManager> _authManagerMock;
        private CommentControllerAPI _controller;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IUserRepository> _userRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            this._commentServiceMock = new Mock<ICommentService>();
            this._postServiceMock = new Mock<IPostService>();
            this._modelMapperMock = new Mock<ModelMapper>();
            this._httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            this._configurationMock = new Mock<IConfiguration>();
            this._userRepositoryMock = new Mock<IUserRepository>();

            this._authManagerMock = new Mock<AuthManager>(this._userRepositoryMock.Object, this._configurationMock.Object);

            this._controller = new CommentControllerAPI(this._commentServiceMock.Object, this._postServiceMock.Object, this._authManagerMock.Object, this._modelMapperMock.Object);

            // Mock the HttpContext and set up a ClaimsPrincipal
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "User1"),
            }, "mock"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(user);

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            this._controller.ControllerContext = controllerContext;
        }

        [TestMethod]
        public void ReturnAllPostComments_When_ValidPostId()
        {
            // Arrange
            int postId = 1;
            Post post = new Post { Id = postId, Comments = new List<Comment> { new Comment { Id = 1, Content = "Comment 1" } } };
            List<CommentReplyResponseDTO> commentsDto = new List<CommentReplyResponseDTO> { new CommentReplyResponseDTO { Content = "Comment 1" } };

            this._postServiceMock.Setup(x => x.GetPost(postId)).Returns(post);
            this._modelMapperMock.Setup(x => x.Map(It.IsAny<List<Comment>>())).Returns(commentsDto);

            // Act
            var result = this._controller.GetAllPostComments(postId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(commentsDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnPagedReplies_When_ValidParentCommentId()
        {
            // Arrange
            int parentCommentId = 1;
            PagedResult<Comment> pagedReplies = new PagedResult<Comment>
            {
                Items = new List<Comment> { new Comment { Id = 2, Content = "Reply 1" } },
                Metadata = new PaginationMetadata { TotalCount = 1, PageSize = 10, CurrentPage = 1, TotalPages = 1 }
            };
            List<ReplyResponseDTO> repliesDto = new List<ReplyResponseDTO> { new ReplyResponseDTO { Content = "Reply 1" } };

            this._commentServiceMock.Setup(x => x.GetPagedReplies(parentCommentId, 1, 10)).Returns(pagedReplies);
            this._modelMapperMock.Setup(x => x.MapReplyResponse(pagedReplies.Items)).Returns(repliesDto);

            // Act
            var result = this._controller.GetReplies(parentCommentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var response = okResult.Value as dynamic;
            Assert.IsNotNull(response);
            
            
        }

        [TestMethod]
        public void ReturnCreatedComment_When_ValidComment()
        {
            // Arrange
            int postId = 1;
            CommentRequestDTO commentRequestDto = new CommentRequestDTO { Content = "New Comment" };
            Post post = new Post { Id = postId };
            Comment comment = new Comment { Id = 1, Content = "New Comment", PostID = postId };
            CommentReplyResponseDTO commentDto = new CommentReplyResponseDTO { Content = "New Comment" };

            this._postServiceMock.Setup(x => x.GetPost(postId)).Returns(post);
            this._modelMapperMock.Setup(x => x.Map(commentRequestDto, postId, 0)).Returns(comment);
            this._commentServiceMock.Setup(x => x.CreateComment(comment, It.IsAny<User>())).Returns(comment);
            this._modelMapperMock.Setup(x => x.Map(comment)).Returns(commentDto);

            // Act
            var result = this._controller.CreateComment(postId, commentRequestDto);

            // Assert
            var createdResult = result as ObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
            Assert.AreEqual(commentDto, createdResult.Value);
        }

        [TestMethod]
        public void ReturnCreatedReply_When_ValidReply()
        {
            // Arrange
            int parentCommentId = 1;
            CommentRequestDTO replyRequestDto = new CommentRequestDTO { Content = "New Reply" };
            Comment reply = new Comment { Id = 2, Content = "New Reply", ParentCommentID = parentCommentId };
            CommentReplyResponseDTO replyDto = new CommentReplyResponseDTO { Content = "New Reply" };

            this._modelMapperMock.Setup(x => x.Map(replyRequestDto, 0, 0)).Returns(reply);
            this._commentServiceMock.Setup(x => x.CreateReply(reply, parentCommentId, It.IsAny<User>())).Returns(reply);
            this._modelMapperMock.Setup(x => x.Map(reply)).Returns(replyDto);

            // Act
            var result = this._controller.CreateReply(parentCommentId, replyRequestDto);

            // Assert
            var createdResult = result as ObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
            Assert.AreEqual(replyDto, createdResult.Value);
        }

        [TestMethod]
        public void ReturnUpdatedComment_When_ValidComment()
        {
            // Arrange
            int commentId = 1;
            int postId = 1;
            CommentRequestDTO commentRequestDto = new CommentRequestDTO { Content = "Updated Comment" };
            Comment comment = new Comment { Id = commentId, Content = "Updated Comment", PostID = postId };
            CommentReplyResponseDTO commentDto = new CommentReplyResponseDTO { Content = "Updated Comment" };
            Post post = new Post { Id = postId, Comments = new List<Comment> { comment } };

            this._postServiceMock.Setup(x => x.GetPost(postId)).Returns(post);
            this._modelMapperMock.Setup(x => x.Map(commentRequestDto, 0,commentId)).Returns(comment);
            this._commentServiceMock.Setup(x => x.UpdateComment(commentId, comment, It.IsAny<User>())).Returns(comment);
            this._modelMapperMock.Setup(x => x.Map(comment)).Returns(commentDto);

            // Act
            var result = this._controller.UpdateComment(commentId, commentRequestDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            
        }

        [TestMethod]
        public void ReturnOk_When_DeletingValidComment()
        {
            // Arrange
            int commentId = 1;

            this._commentServiceMock.Setup(x => x.DeleteComment(commentId, It.IsAny<User>())).Returns(true);

            // Act
            var result = this._controller.DeleteComment(commentId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(true, okResult.Value);
        }
    }
}
