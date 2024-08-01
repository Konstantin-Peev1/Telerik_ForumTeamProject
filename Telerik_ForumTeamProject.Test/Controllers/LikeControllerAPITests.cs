using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Controllers;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Tests.Controllers.LikeControllerAPITests
{
    [TestClass]
    public class LikeControllerAPITests
    {
        private Mock<ILikeService> _likeServiceMock;
        private Mock<ModelMapper> _modelMapperMock;
        private Mock<AuthManager> _authManagerMock;
        private LikeControllerAPI _controller;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IUserRepository> _userRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            this._likeServiceMock = new Mock<ILikeService>();
            this._modelMapperMock = new Mock<ModelMapper>();
            this._httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            this._configurationMock = new Mock<IConfiguration>();
            this._userRepositoryMock = new Mock<IUserRepository>();

            this._authManagerMock = new Mock<AuthManager>(this._userRepositoryMock.Object, this._configurationMock.Object);

            this._controller = new LikeControllerAPI(this._likeServiceMock.Object, this._modelMapperMock.Object, this._authManagerMock.Object);

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
        public void ReturnOk_When_AddingLikeValid()
        {
            // Arrange
            int postId = 1;
            User user = new User { ID = 1, UserName = "User1" };
            Like like = new Like { Id = 1, PostID = postId, UserId = user.ID };
            LikeResponseDTO likeDto = new LikeResponseDTO { UserName = user.UserName };

            this._authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            this._likeServiceMock.Setup(x => x.Create(postId, user)).Returns(like);
            this._modelMapperMock.Setup(x => x.Map(like)).Returns(likeDto);

            // Act
            var result = this._controller.AddRemoveLike(postId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(likeDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnUnauthorized_When_UserNotAuthenticated()
        {
            // Arrange
            int postId = 1;

            this._authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Throws(new AuthorisationExcpetion("Invalid credentials"));

            // Act
            var result = this._controller.AddRemoveLike(postId);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.IsNotNull(unauthorizedResult);
            Assert.AreEqual(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
        }

        [TestMethod]
        public void ReturnConflict_When_LikeAlreadyExists()
        {
            // Arrange
            int postId = 1;
            User user = new User { ID = 1, UserName = "User1" };

            this._authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            this._likeServiceMock.Setup(x => x.Create(postId, user)).Throws(new DuplicateEntityException("Like already exists"));

            // Act
            var result = this._controller.AddRemoveLike(postId);

            // Assert
            var conflictResult = result as ConflictObjectResult;
            Assert.IsNotNull(conflictResult);
            Assert.AreEqual(StatusCodes.Status409Conflict, conflictResult.StatusCode);
        }
    }
}
