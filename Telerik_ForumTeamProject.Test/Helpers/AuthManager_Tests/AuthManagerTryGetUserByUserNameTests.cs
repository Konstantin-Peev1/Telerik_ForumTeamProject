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
    public class TryGetUserByUserName_Should
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
        public void ReturnCorrectUser_When_UserExists()
        {
            // Arrange
            var user = new User { UserName = "testUser" };
            _userRepositoryMock.Setup(r => r.GetByInformationUsername("testUser")).Returns(user);

            // Act
            var result = _authManager.TryGetUserByUserName("testUser");

            // Assert
            Assert.AreEqual(user, result);
        }

        [TestMethod]
        [ExpectedException(typeof(EntityNotFoundException))]
        public void ThrowEntityNotFoundException_When_UserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByInformationUsername(It.IsAny<string>())).Returns((User)null);

            // Act
            _authManager.TryGetUserByUserName("nonExistentUser");

            // Assert is handled by ExpectedException
        }
    }
}
