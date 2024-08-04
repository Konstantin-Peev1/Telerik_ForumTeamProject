using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using Telerik_ForumTeamProject.Controllers.MVC;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ViewModels;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Tests.Controllers.MVC
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<IPostService> _postServiceMock;
        private Mock<IUserService> _userServiceMock;
        private HomeController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _postServiceMock = new Mock<IPostService>();
            _userServiceMock = new Mock<IUserService>();

            _controller = new HomeController(_postServiceMock.Object, _userServiceMock.Object);

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

            _controller.ControllerContext = controllerContext;
        }

        [TestMethod]
        public void Index_ShouldReturnViewWithModel_WhenUserIsAuthenticatedAndMostCommented()
        {
            // Arrange
            var topCommentedPosts = new List<Post> { new Post { Id = 1, Title = "Post 1" } };
            _postServiceMock.Setup(x => x.GetTop10Commented()).Returns(topCommentedPosts);
            _userServiceMock.Setup(x => x.GetByInformation(It.IsAny<string>())).Returns(new User { UserName = "User1" });

            // Act
            var result = _controller.Index("mostCommented") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as HomeViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("mostCommented", model.ViewOption);
            Assert.IsTrue(model.IsAuthenticated);
            Assert.AreEqual(1, model.TopCommentedPosts.Count);
        }

        [TestMethod]
        public void Index_ShouldReturnViewWithModel_WhenUserIsAuthenticatedAndRecent()
        {
            // Arrange
            var recentPosts = new List<Post> { new Post { Id = 1, Title = "Post 1" } };
            _postServiceMock.Setup(x => x.GetTop10Recent()).Returns(recentPosts);
            _userServiceMock.Setup(x => x.GetByInformation(It.IsAny<string>())).Returns(new User { UserName = "User1" });

            // Act
            var result = _controller.Index("recent") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as HomeViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("recent", model.ViewOption);
            Assert.IsTrue(model.IsAuthenticated);
            Assert.AreEqual(1, model.RecentPosts.Count);
        }

        [TestMethod]
        public void Index_ShouldReturnViewWithModel_WhenUserIsNotAuthenticated()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            _postServiceMock.Setup(x => x.GetTop10Commented()).Returns(new List<Post>());
            _postServiceMock.Setup(x => x.GetTop10Recent()).Returns(new List<Post>());

            // Act
            var result = _controller.Index("mostCommented") as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as HomeViewModel;
            Assert.IsNotNull(model);
            //Assert.IsFalse(model.IsAuthenticated);
        }

        [TestMethod]
        public void Error_ShouldReturnView()
        {
            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
