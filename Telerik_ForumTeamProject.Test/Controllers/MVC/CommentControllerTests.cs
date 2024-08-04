using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Controllers.MVC;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Services.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Tests.Controllers.MVC
{
    [TestClass]
    public class CommentControllerTests
    {
        private Mock<ICommentService> _commentServiceMock;
        private Mock<AuthManager> _authManagerMock;
        private Mock<ModelMapper> _modelMapperMock;
        private CommentController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _commentServiceMock = new Mock<ICommentService>();
            _modelMapperMock = new Mock<ModelMapper>();
            var configurationMock = new Mock<IConfiguration>();
            var userRepositoryMock = new Mock<IUserRepository>();
            _authManagerMock = new Mock<AuthManager>(userRepositoryMock.Object, configurationMock.Object);

            _controller = new CommentController(
                _commentServiceMock.Object,
                _authManagerMock.Object,
                _modelMapperMock.Object
            );

            // Setup HttpContext with ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"), // Assuming user ID is a string
                new Claim(ClaimTypes.Role, "User")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            _controller.ControllerContext = controllerContext;

            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns((string userName) =>
            {
                return new User
                {
                    ID = 1,
                    UserName = userName,
                    FirstName = "Test",
                    LastName = "User",
                    Email = "testuser@example.com",
                    ProfilePictureUrl = "http://example.com/pic.jpg"
                };
            });
        }

        [TestMethod]
        public void EditComment_Get_ShouldReturnView_WhenCommentExists()
        {
            // Arrange
            var comment = new Comment { Id = 1, Content = "Test Comment", UserID = 1, PostID = 1 };
            _commentServiceMock.Setup(x => x.GetComment(It.IsAny<int>())).Returns(comment);

            // Act
            var result = _controller.EditComment(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(CommentRequestDTO));
            Assert.AreEqual("Test Comment", ((CommentRequestDTO)result.Model).Content);
        }

        [TestMethod]
        public void EditComment_Get_ShouldReturnForbid_WhenUserIsNotOwnerOrAdmin()
        {
            // Arrange
            var comment = new Comment { Id = 1, Content = "Test Comment", UserID = 2, PostID = 1 };
            _commentServiceMock.Setup(x => x.GetComment(It.IsAny<int>())).Returns(comment);

            // Act
            var result = _controller.EditComment(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public void EditComment_Post_ShouldReturnRedirectToAction_WhenCommentIsUpdated()
        {
            // Arrange
            var commentRequest = new CommentRequestDTO { Content = "Updated Comment" };
            var comment = new Comment { Id = 1, Content = "Test Comment", UserID = 1, PostID = 1 };
            var updatedComment = new Comment { Id = 1, Content = "Updated Comment", UserID = 1, PostID = 1 };

            _commentServiceMock.Setup(x => x.GetComment(It.IsAny<int>())).Returns(comment);
            _modelMapperMock.Setup(x => x.Map(commentRequest, 0, It.IsAny<int>())).Returns(updatedComment);
            _commentServiceMock.Setup(x => x.UpdateComment(It.IsAny<int>(), It.IsAny<Comment>(), It.IsAny<User>())).Returns(updatedComment);

            // Act
            var result = _controller.EditComment(1, commentRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("GetPost", result.ActionName);
            Assert.AreEqual("Post", result.ControllerName);
            Assert.AreEqual(1, result.RouteValues["id"]);
        }

        [TestMethod]
        public void EditComment_Post_ShouldReturnView_WhenCommentRequestIsInvalid()
        {
            // Arrange
            var commentRequest = new CommentRequestDTO { Content = "" };
            var comment = new Comment { Id = 1, Content = "Test Comment", UserID = 1, PostID = 1 };

            _commentServiceMock.Setup(x => x.GetComment(It.IsAny<int>())).Returns(comment);

            // Act
            var result = _controller.EditComment(1, commentRequest) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as CommentRequestDTO;
            Assert.AreEqual("", model.Content);
            Assert.AreEqual(1, comment.PostID);
        }

        [TestMethod]
        public void DeleteComment_ShouldReturnRedirectToAction_WhenCommentIsDeleted()
        {
            // Arrange
            var comment = new Comment { Id = 1, PostID = 1, UserID = 1 };

            _commentServiceMock.Setup(x => x.GetComment(It.IsAny<int>())).Returns(comment);
            _commentServiceMock.Setup(x => x.DeleteComment(It.IsAny<int>(), It.IsAny<User>())).Returns(true);

            // Act
            var result = _controller.DeleteComment(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("GetPost", result.ActionName);
            Assert.AreEqual("Post", result.ControllerName);
            Assert.AreEqual(1, result.RouteValues["id"]);
        }
    }
}
