using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Controllers.MVC;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Services.Contracts;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Models.ServiceModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telerik_ForumTeamProject.Models.ViewModels;

namespace Telerik_ForumTeamProject.Tests.Controllers.MVC
{
    [TestClass]
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IPostService> _postServiceMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private Mock<ModelMapper> _modelMapperMock;
        private Mock<ILogger<UserController>> _loggerMock;
        private UserController _controller;
        private Mock<AuthManager> _authManagerMock;
        private Mock<IUserRepository> _userRepositoryMock;

        [TestInitialize]
        public void Initialize()
        {
            _userServiceMock = new Mock<IUserService>();
            _postServiceMock = new Mock<IPostService>();
            _cloudinaryServiceMock = new Mock<ICloudinaryService>();
            _modelMapperMock = new Mock<ModelMapper>();
            _loggerMock = new Mock<ILogger<UserController>>();
            var configurationMock = new Mock<IConfiguration>();
            _userRepositoryMock = new Mock<IUserRepository>();

            // Setup AuthManager mock
            _authManagerMock = new Mock<AuthManager>(_userRepositoryMock.Object, configurationMock.Object);

            _controller = new UserController(
                _userServiceMock.Object,
                _postServiceMock.Object,
                _cloudinaryServiceMock.Object,
                _modelMapperMock.Object,
                _authManagerMock.Object
            );

            // Setup TempData
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;

            configurationMock.SetupGet(x => x["Jwt:Key"]).Returns("thisisaverysecurekeyformyjwt12345");
            configurationMock.SetupGet(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            configurationMock.SetupGet(x => x["Jwt:Audience"]).Returns("TestAudience");

            // Setup HttpContext with ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "John")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);

            var responseMock = new Mock<HttpResponse>();
            var cookiesMock = new Mock<IResponseCookies>();
            responseMock.Setup(r => r.Cookies).Returns(cookiesMock.Object);
            httpContextMock.Setup(x => x.Response).Returns(responseMock.Object);

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            _controller.ControllerContext = controllerContext;

            // Mock the GetByInformationUsername to return a user
            _userRepositoryMock.Setup(x => x.GetByInformationUsername(It.IsAny<string>())).Returns((string username) =>
            {
                return new User
                {
                    UserName = username,
                    ID = 1,
                    FirstName = "Test",
                    LastName = "User",
                    Email = "testuser@example.com",
                    ProfilePictureUrl = "http://example.com/pic.jpg"
                };
            });

            _userServiceMock.Setup(x => x.GetByInformationUsername(It.IsAny<string>())).Returns((string username) =>
            {
                return new User
                {
                    UserName = username,
                    ID = 1,
                    FirstName = "Test",
                    LastName = "User",
                    Email = "testuser@example.com",
                    ProfilePictureUrl = "http://example.com/pic.jpg"
                };
            });
        }

        [TestMethod]
        public void ReturnView_WhenLoginIsCalled()
        {
            // Act
            var result = _controller.Login() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(LogInRequestDTO));
        }

        [TestMethod]
        public void ReturnViewWithModelError_WhenLoginRequestIsNull()
        {
            // Act
            var result = _controller.Login(null) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.ModelState.ContainsKey(string.Empty));
            Assert.AreEqual("Invalid login request", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ReturnRedirectToHome_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginRequest = new LogInRequestDTO { UserName = "testuser", Password = "password" };
            var user = new User { UserName = "testuser", Password = "password" };

            _authManagerMock.Setup(x => x.Authenticate(loginRequest.UserName, loginRequest.Password)).Returns(user);
            _authManagerMock.Setup(x => x.Generate(user)).Returns("fake-token");

            // Act
            var result = _controller.Login(loginRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }

        [TestMethod]
        public void ReturnViewWithModelError_WhenLoginFails()
        {
            // Arrange
            var loginRequest = new LogInRequestDTO { UserName = "testuser", Password = "wrongpassword" };

            // Simulate the authentication failure
            _authManagerMock.Setup(x => x.Authenticate(loginRequest.UserName, loginRequest.Password)).Throws(new AuthorisationExcpetion("Invalid credentials!"));

            // Act
            var result = _controller.Login(loginRequest) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.ModelState.ContainsKey(string.Empty));
            Assert.AreEqual("Invalid credentials!", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void ReturnView_WhenRegisterIsCalled()
        {
            // Act
            var result = _controller.Register() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(UserRequestDTO));
        }

        [TestMethod]
        public void ReturnRedirectToHome_WhenRegisterIsSuccessful()
        {
            // Arrange
            var registerRequest = new UserRequestDTO { UserName = "newuser", Password = "password", Email = "email@example.com", FirstName = "Kosio", LastName = "Kosio" };
            var user = new User { UserName = "newuser", Password = "password", Email = "email@example.com", FirstName = "Kosio", LastName = "Kosio" };

            _modelMapperMock.Setup(x => x.Map(registerRequest)).Returns(user);
            _userServiceMock.Setup(x => x.CreateUser(It.IsAny<User>())).Returns(user);
            _authManagerMock.Setup(x => x.Generate(user)).Returns("fake-token");

            // Act
            var result = _controller.Register(registerRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }

        [TestMethod]
        public void ReturnConflict_When_RegisteringDuplicateUser()
        {
            // Arrange
            UserRequestDTO registerRequest = new UserRequestDTO { FirstName = "New", LastName = "User", Email = "newuser@example.com", Password = "password" };
            User userEntity = new User { ID = 1, FirstName = "New", LastName = "User", Email = "newuser@example.com" };

            _modelMapperMock.Setup(x => x.Map(registerRequest)).Returns(userEntity);
            _userServiceMock.Setup(x => x.CreateUser(It.IsAny<User>())).Throws(new DuplicateEntityException(""));

            // Act
            var result = _controller.Register(registerRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ConflictObjectResult));
        }

        [TestMethod]
        public void ReturnRedirectToHome_WhenLogoutIsCalled()
        {
            // Act
            var result = _controller.Logout() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.IsFalse((bool)_controller.TempData["IsAuthenticated"]);
        }

        [TestMethod]
        public void ReturnViewModel_WhenSearchIsCalled()
        {
            // Arrange
            string query = "test";
            var users = new List<User> { new User { UserName = "testuser" } };
            _userServiceMock.Setup(x => x.SearchUsers(query)).Returns(users.AsQueryable());

            // Act
            var result = _controller.Search(query) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as UserSearchViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(query, model.Query);
            Assert.AreEqual(users.Count, model.Users.Count);
        }

        [TestMethod]
        public void ReturnErrorView_WhenUserNotFoundInSearch()
        {
            // Arrange
            string query = "nonexistent";
            _userServiceMock.Setup(x => x.SearchUsers(query)).Throws(new EntityNotFoundException("User not found"));

            // Act
            var result = _controller.Search(query) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("User not found", _controller.TempData["ErrorMessage"]);
            var model = result.Model as UserSearchViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(query, model.Query);
            Assert.AreEqual(0, model.Users.Count);
        }

        [TestMethod]
        public void ReturnUserDetails_WhenUserExists()
        {
            // Arrange
            string username = "testuser";
            var user = new User { UserName = username };
            _userServiceMock.Setup(x => x.GetByInformationUsername(username)).Returns(user);

            // Act
            var result = _controller.Details(username) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user, result.Model);
        }

        [TestMethod]
        public void ReturnNotFoundView_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "nonexistent";
            _userServiceMock.Setup(x => x.GetByInformationUsername(username)).Throws(new EntityNotFoundException("User not found"));

            // Act
            var result = _controller.Details(username) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            
            Assert.AreEqual("Error", result.ViewName);
            Assert.AreEqual("User not found", _controller.ViewData["ErrorMessage"]);
        }

        [TestMethod]
        public async Task ReturnOk_When_UploadProfilePictureValid()
        {
            // Arrange
            int userId = 1;
            var fileMock = new Mock<IFormFile>();
            var content = "Fake file content";
            var fileName = "file.png";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);

            var user = new User { ID = 1, ProfilePictureUrl = "", UserName = "John" };
            var uploadResult = new CloudinaryUploadResult { Url = "http://example.com/file.png" };
            var updatedUser = new User { ID = userId, ProfilePictureUrl = "http://example.com/file.png", UserName = "John" };

            _cloudinaryServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);
            _userServiceMock.Setup(x => x.GetByInformationUsername(It.IsAny<string>())).Returns(user);  // Ensure this returns a valid user
            _userServiceMock.Setup(x => x.UpdateProfilePicture(userId, uploadResult.Url)).Returns(updatedUser);

            // Act
            var result = await _controller.UpdateProfilePicture(fileMock.Object);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual(updatedUser.UserName, redirectResult.RouteValues["username"]);
        }

        [TestMethod]
        public void ReturnBlockedUser_When_BlockingValidUser()
        {
            // Arrange
            string userName = "User1";
            User userEntity = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", UserName = userName };
            User blockedUser = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsBlocked = true, UserName = userName };

            _userServiceMock.Setup(x => x.GetByInformationUsername(userName)).Returns(userEntity);
            _userServiceMock.Setup(x => x.BlockUser(userEntity)).Returns(blockedUser);

            // Act
            var result = _controller.BlockUser(userName);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual(userName, redirectResult.RouteValues["username"]);
            Assert.AreEqual("User has been blocked successfully.", _controller.TempData["Success"]);
        }

        [TestMethod]
        public void ReturnUnblockedUser_When_UnblockingValidUser()
        {
            // Arrange
            string userName = "User1";
            User userEntity = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsBlocked = true, UserName = userName };
            User unblockedUser = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsBlocked = false, UserName = userName };

            _userServiceMock.Setup(x => x.GetByInformationUsername(userName)).Returns(userEntity);
            _userServiceMock.Setup(x => x.UnBlockUser(userEntity)).Returns(unblockedUser);

            // Act
            var result = _controller.UnBlockUser(userName);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual(userName, redirectResult.RouteValues["username"]);
            Assert.AreEqual("User has been unblocked successfully.", _controller.TempData["Success"]);
        }

        [TestMethod]
        public void ReturnNewAdmin_When_MakingValidUserAdmin()
        {
            // Arrange
            string userName = "User1";
            User userEntity = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", UserName = userName };
            User newAdmin = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsAdmin = true, UserName = userName };

            _userServiceMock.Setup(x => x.GetByInformationUsername(userName)).Returns(userEntity);
            _userServiceMock.Setup(x => x.MakeAdmin(userEntity)).Returns(newAdmin);

            // Act
            var result = _controller.MakeAdmin(userName);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual(userName, redirectResult.RouteValues["username"]);
            Assert.AreEqual("User been promoted to admin successfully.", _controller.TempData["Success"]);
        }

        [TestMethod]
        public void ReturnDemotedUser_When_DemotingAdmin()
        {
            // Arrange
            string userName = "User1";
            User userEntity = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsAdmin = true, UserName = userName };
            User demotedUser = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsAdmin = false, UserName = userName };

            _userServiceMock.Setup(x => x.GetByInformationUsername(userName)).Returns(userEntity);
            _userServiceMock.Setup(x => x.MakeUser(userEntity)).Returns(demotedUser);

            // Act
            var result = _controller.DemoteAdmin(userName);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual(userName, redirectResult.RouteValues["username"]);
            Assert.AreEqual("Admin been demoted to user successfully.", _controller.TempData["Success"]);
        }

        [TestMethod]
        public async Task ReturnError_When_ProfilePictureIsNull()
        {
            // Act
            var result = await _controller.UpdateProfilePicture(null) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("Please select a valid image file.", _controller.TempData["Error"]);
        }

        [TestMethod]
        public async Task ReturnError_When_ProfilePictureIsEmpty()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            // Act
            var result = await _controller.UpdateProfilePicture(fileMock.Object) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("Please select a valid image file.", _controller.TempData["Error"]);
        }

        [TestMethod]
        public async Task ReturnError_When_FileExtensionIsNotAllowed()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("file.txt");
            fileMock.Setup(f => f.Length).Returns(1024);

            // Act
            var result = await _controller.UpdateProfilePicture(fileMock.Object) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("Invalid file format. Only .jpg, .jpeg, and .png are allowed.", _controller.TempData["Error"]);
        }

        [TestMethod]
        public async Task ReturnError_When_UploadImageFails()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("file.jpg");
            fileMock.Setup(f => f.Length).Returns(1024);

            _cloudinaryServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync((CloudinaryUploadResult)null);

            // Act
            var result = await _controller.UpdateProfilePicture(fileMock.Object) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("Error uploading image.", _controller.TempData["Error"]);
        }

        [TestMethod]
        public async Task ReturnError_When_EntityNotFoundExceptionIsThrown()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("file.jpg");
            fileMock.Setup(f => f.Length).Returns(1024);

            var uploadResult = new CloudinaryUploadResult { Url = "http://example.com/file.jpg" };
            _cloudinaryServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);
            _userServiceMock.Setup(x => x.UpdateProfilePicture(It.IsAny<int>(), uploadResult.Url)).Throws(new EntityNotFoundException("User not found"));

            // Act
            var result = await _controller.UpdateProfilePicture(fileMock.Object) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("User not found", _controller.TempData["Error"]);
        }

        [TestMethod]
        public async Task ReturnError_When_GeneralExceptionIsThrown()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("file.jpg");
            fileMock.Setup(f => f.Length).Returns(1024);

            var uploadResult = new CloudinaryUploadResult { Url = "http://example.com/file.jpg" };
            _cloudinaryServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);
            _userServiceMock.Setup(x => x.UpdateProfilePicture(It.IsAny<int>(), uploadResult.Url)).Throws(new Exception("General error"));

            // Act
            var result = await _controller.UpdateProfilePicture(fileMock.Object) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ActionName);
            Assert.AreEqual("An error occurred while updating the profile picture. Please try again.", _controller.TempData["Error"]);
        }

        [TestMethod]
        public async Task ReturnRedirectToDetails_When_UpdateProfilePictureIsSuccessful()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "Fake file content";
            var fileName = "file.png";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);

            var user = new User { ID = 1, ProfilePictureUrl = "", UserName = "John" };
            var uploadResult = new CloudinaryUploadResult { Url = "http://example.com/file.png" };
            var updatedUser = new User { ID = 1, ProfilePictureUrl = "http://example.com/file.png", UserName = "John" };

            _cloudinaryServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);
            _userServiceMock.Setup(x => x.GetByInformationUsername(It.IsAny<string>())).Returns(user);
            _userServiceMock.Setup(x => x.UpdateProfilePicture(user.ID, uploadResult.Url)).Returns(updatedUser);

            // Act
            var result = await _controller.UpdateProfilePicture(fileMock.Object);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Details", redirectResult.ActionName);
            Assert.AreEqual(updatedUser.UserName, redirectResult.RouteValues["username"]);
            Assert.AreEqual("Profile picture updated successfully.", _controller.TempData["Success"]);
        }
    }
}
