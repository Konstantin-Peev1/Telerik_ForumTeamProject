using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Controllers;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Telerik_ForumTeamProject.Models.ServiceModel;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Tests.Controllers.UserControllerAPITests
{
    [TestClass]
    public class UserControllerAPITests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<ModelMapper> _modelMapperMock;
        private Mock<AuthManager> _authManagerMock;
        private Mock<ICloudinaryService> _cloudinaryServiceMock;
        private UserControllerAPI _controller;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IUserRepository> _userRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            this._userServiceMock = new Mock<IUserService>();
            this._modelMapperMock = new Mock<ModelMapper>();
            this._cloudinaryServiceMock = new Mock<ICloudinaryService>();
            this._httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            this._configurationMock = new Mock<IConfiguration>();
            this._userRepositoryMock = new Mock<IUserRepository>();

            this._authManagerMock = new Mock<AuthManager>(this._userRepositoryMock.Object, this._configurationMock.Object);

            this._controller = new UserControllerAPI(this._userServiceMock.Object, this._modelMapperMock.Object, this._authManagerMock.Object, this._cloudinaryServiceMock.Object);

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
        public void ReturnUser_When_InformationIsValid()
        {
            // Arrange
            string information = "User1";
            User userEntity = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com" };
            UserResponseDTO userResponseDto = new UserResponseDTO { FirstName = "John", LastName = "Doe", Email = "user1@example.com" };

            this._userServiceMock.Setup(x => x.GetByInformation(information)).Returns(userEntity);
            this._modelMapperMock.Setup(x => x.Map(userEntity)).Returns(userResponseDto);

            // Act
            var result = this._controller.Get(information);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(userResponseDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnNotFound_When_UserDoesNotExist()
        {
            // Arrange
            string information = "NonExistentUser";
            this._userServiceMock.Setup(x => x.GetByInformation(information)).Throws(new EntityNotFoundException(""));

            // Act
            var result = this._controller.Get(information);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public void ReturnUpdatedUser_When_UpdatingValidUser()
        {
            // Arrange
            int userId = 1;
            UserRequestDTO userRequestDto = new UserRequestDTO { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" };
            User userEntity = new User { ID = 1, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" };
            UserResponseDTO userResponseDto = new UserResponseDTO { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" };

            this._userServiceMock.Setup(x => x.UpdateUser(It.IsAny<User>(), It.IsAny<User>(), userId)).Returns(userEntity);
            this._modelMapperMock.Setup(x => x.Map(userRequestDto)).Returns(userEntity);
            this._modelMapperMock.Setup(x => x.Map(userEntity)).Returns(userResponseDto);

            // Act
            var result = this._controller.Update(userId, userRequestDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(userResponseDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnUnauthorized_When_InvalidCredentials()
        {
            // Arrange
            LogInRequestDTO loginRequest = new LogInRequestDTO { UserName = "InvalidUser", Password = "InvalidPassword" };
            this._authManagerMock.Setup(x => x.Authenticate(loginRequest.UserName, loginRequest.Password)).Throws(new AuthorisationExcpetion(""));

            // Act
            var result = this._controller.Login(loginRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
        }

        [TestMethod]
        public void ReturnCreatedUser_When_RegisteringValidUser()
        {
            // Arrange
            UserRequestDTO registerRequest = new UserRequestDTO { FirstName = "New", LastName = "User", Email = "newuser@example.com", Password = "password" };
            User userEntity = new User { ID = 1, FirstName = "New", LastName = "User", Email = "newuser@example.com" };
            UserResponseDTO userResponseDto = new UserResponseDTO { FirstName = "New", LastName = "User", Email = "newuser@example.com" };

            this._userServiceMock.Setup(x => x.CreateUser(It.IsAny<User>())).Returns(userEntity);
            this._modelMapperMock.Setup(x => x.Map(registerRequest)).Returns(userEntity);
            this._modelMapperMock.Setup(x => x.Map(userEntity)).Returns(userResponseDto);

            // Act
            var result = this._controller.Register(registerRequest);

            // Assert
            var createdResult = result as ObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
            Assert.AreEqual(userResponseDto, createdResult.Value);
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

            var uploadResult = new CloudinaryUploadResult { Url = "http://example.com/file.png" };
            var updatedUser = new User { ID = userId, ProfilePictureUrl = "http://example.com/file.png" };

            this._cloudinaryServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);
            this._userServiceMock.Setup(x => x.UpdateProfilePicture(userId, uploadResult.Url)).Returns(updatedUser);

            // Act
            var result = await this._controller.UploadProfilePicture(userId, fileMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            //Assert.AreEqual(new { updatedUser.ProfilePictureUrl }, okResult.Value);
        }

        [TestMethod]
        public void ReturnBlockedUser_When_BlockingValidUser()
        {
            // Arrange
            string userName = "User1";
            User userEntity = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com" };
            User blockedUser = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsBlocked = true };
            UserResponseDTO userResponseDto = new UserResponseDTO { FirstName = "John", LastName = "Doe", Email = "user1@example.com" };

            this._userServiceMock.Setup(x => x.GetByInformation(userName)).Returns(userEntity);
            this._userServiceMock.Setup(x => x.BlockUser(userEntity)).Returns(blockedUser);
            this._modelMapperMock.Setup(x => x.Map(blockedUser)).Returns(userResponseDto);

            // Act
            var result = this._controller.Block(userName);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(userResponseDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnUnblockedUser_When_UnblockingValidUser()
        {
            // Arrange
            string userName = "User1";
            User userEntity = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsBlocked = true };
            User unblockedUser = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsBlocked = false };
            UserResponseDTO userResponseDto = new UserResponseDTO { FirstName = "John", LastName = "Doe", Email = "user1@example.com" };

            this._userServiceMock.Setup(x => x.GetByInformation(userName)).Returns(userEntity);
            this._userServiceMock.Setup(x => x.UnBlockUser(userEntity)).Returns(unblockedUser);
            this._modelMapperMock.Setup(x => x.Map(unblockedUser)).Returns(userResponseDto);

            // Act
            var result = this._controller.UnBlock(userName);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(userResponseDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnNewAdmin_When_MakingValidUserAdmin()
        {
            // Arrange
            string userName = "User1";
            User userEntity = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com" };
            User newAdmin = new User { ID = 1, FirstName = "John", LastName = "Doe", Email = "user1@example.com", IsAdmin = true };
            UserResponseDTO userResponseDto = new UserResponseDTO { FirstName = "John", LastName = "Doe", Email = "user1@example.com" };

            this._userServiceMock.Setup(x => x.GetByInformation(userName)).Returns(userEntity);
            this._userServiceMock.Setup(x => x.MakeAdmin(userEntity)).Returns(newAdmin);
            this._modelMapperMock.Setup(x => x.Map(newAdmin)).Returns(userResponseDto);

            // Act
            var result = this._controller.MakeAdmin(userName);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(userResponseDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnConflict_When_RegisteringDuplicateUser()
        {
            // Arrange
            UserRequestDTO registerRequest = new UserRequestDTO { FirstName = "New", LastName = "User", Email = "newuser@example.com", Password = "password" };
            User userEntity = new User { ID = 1, FirstName = "New", LastName = "User", Email = "newuser@example.com" };

            this._modelMapperMock.Setup(x => x.Map(registerRequest)).Returns(userEntity);
            this._userServiceMock.Setup(x => x.CreateUser(It.IsAny<User>())).Throws(new DuplicateEntityException(""));

            // Act
            var result = this._controller.Register(registerRequest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ConflictObjectResult));
        }
    }
}
