using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Microsoft.Extensions.Configuration;

namespace Telerik_ForumTeamProject.Test.Helpers.AuthManager_Tests
{
    [TestClass]
    public class Authenticate_Should
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IConfiguration> _configurationMock;
        private AuthManager _authManager;

        [TestInitialize]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _authManager = new AuthManager(_userRepositoryMock.Object, _configurationMock.Object);
        }

        [TestMethod]
        public void ReturnCorrectUser_When_ValidCredentials()
        {
            // Arrange
            var user = new User { UserName = "testUser", Password = BCrypt.Net.BCrypt.HashPassword("password") };
            _userRepositoryMock.Setup(r => r.GetByInformationUsername("testUser")).Returns(user);

            // Act
            var result = _authManager.Authenticate("testUser", "password");

            // Assert
            Assert.AreEqual(user, result);
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorisationExcpetion))]
        public void ThrowAuthorisationException_When_InvalidPassword()
        {
            // Arrange
            var user = new User { UserName = "testUser", Password = BCrypt.Net.BCrypt.HashPassword("wrongPassword") };
            _userRepositoryMock.Setup(r => r.GetByInformationUsername("testUser")).Returns(user);

            // Act
            _authManager.Authenticate("testUser", "password");

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(AuthorisationExcpetion))]
        public void ThrowAuthorisationException_When_UserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByInformationUsername(It.IsAny<string>())).Returns((User)null);

            // Act
            _authManager.Authenticate("nonExistentUser", "password");

            // Assert is handled by ExpectedException
        }
    }
}
