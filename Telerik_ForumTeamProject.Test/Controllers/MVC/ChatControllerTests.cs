using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Telerik_ForumTeamProject.Controllers.MVC;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ViewModels;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Tests.Controllers.MVC
{
    [TestClass]
    public class ChatControllerTests
    {
        private Mock<IChatService> _chatServiceMock;
        private Mock<AuthManager> _authManagerMock;
        private ChatController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _chatServiceMock = new Mock<IChatService>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var configurationMock = new Mock<IConfiguration>();
            _authManagerMock = new Mock<AuthManager>(userRepositoryMock.Object, configurationMock.Object);

            _controller = new ChatController(_chatServiceMock.Object, _authManagerMock.Object);

            // Setup HttpContext with ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "John")
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
        }

        [TestMethod]
        public void Index_ShouldReturnViewWithActiveChats()
        {
            // Arrange
            var activeChats = new List<ChatRoom> { new ChatRoom { Id = 1, Name = "Chat 1" } };
            _chatServiceMock.Setup(x => x.GetActiveChats()).Returns(activeChats);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(activeChats, result.Model);
        }

        [TestMethod]
        public void Room_ShouldReturnViewWithChatRoom_WhenChatRoomExists()
        {
            // Arrange
            var chatRoom = new ChatRoom { Id = 1, Name = "Chat 1" };
            var user = new User { ID = 1, UserName = "John" };
            _chatServiceMock.Setup(x => x.GetChatRoom(It.IsAny<int>())).Returns(chatRoom);
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);

            // Act
            var result = _controller.Room(1) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var viewModel = result.Model as ChatRoomViewModel;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(chatRoom, viewModel.ChatRoom);
            Assert.AreEqual(user, viewModel.CurrentUser);
        }

        [TestMethod]
        public void Room_ShouldReturnNotFound_WhenChatRoomDoesNotExist()
        {
            // Arrange
            _chatServiceMock.Setup(x => x.GetChatRoom(It.IsAny<int>())).Returns((ChatRoom)null);

            // Act
            var result = _controller.Room(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public void SendMessage_ShouldRedirectToRoom()
        {
            // Arrange
            var user = new User { ID = 1, UserName = "John" };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);

            // Act
            var result = _controller.SendMessage(1, "Hello") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Room", result.ActionName);
            Assert.AreEqual(1, result.RouteValues["id"]);
        }

        [TestMethod]
        public void CreateChatRoom_ShouldRedirectToIndex_WhenChatRoomIsCreated()
        {
            // Arrange
            var user = new User { ID = 1, UserName = "John", ChatRooms = new List<ChatRoom>() };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);

            // Act
            var result = _controller.CreateChatRoom("New Chat") as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public void CreateChatRoom_ShouldThrowException_WhenUserHasChatRoom()
        {
            // Arrange
            var user = new User { ID = 1, UserName = "John", ChatRooms = new List<ChatRoom> { new ChatRoom() } };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);

            // Act & Assert
            Assert.ThrowsException<AuthorisationExcpetion>(() => _controller.CreateChatRoom("New Chat"));
        }

        [TestMethod]
        public void DeleteChatRoom_ShouldRedirectToIndex_WhenChatRoomIsDeleted()
        {
            // Arrange
            var user = new User { ID = 1, UserName = "John" };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);

            // Act
            var result = _controller.DeleteChatRoom(1) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public void DeleteChatRoom_ShouldReturnForbid_WhenUserIsUnauthorized()
        {
            // Arrange
            var user = new User { ID = 1, UserName = "John" };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            _chatServiceMock.Setup(x => x.DeleteChatRoom(It.IsAny<int>(), It.IsAny<User>())).Throws(new UnauthorizedAccessException());

            // Act
            var result = _controller.DeleteChatRoom(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public void DeleteChatRoom_ShouldReturnNotFound_WhenChatRoomDoesNotExist()
        {
            // Arrange
            var user = new User { ID = 1, UserName = "John" };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            _chatServiceMock.Setup(x => x.DeleteChatRoom(It.IsAny<int>(), It.IsAny<User>())).Throws(new ArgumentException());

            // Act
            var result = _controller.DeleteChatRoom(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}
