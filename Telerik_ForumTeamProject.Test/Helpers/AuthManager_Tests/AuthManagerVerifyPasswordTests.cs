using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik_ForumTeamProject.Helpers;

namespace Telerik_ForumTeamProject.Test.Helpers.AuthManager_Tests
{
    [TestClass]
    public class VerifyPassword_Should
    {
        private AuthManager _authManager;

        [TestInitialize]
        public void SetUp()
        {
            _authManager = new AuthManager(null, null);
        }

        [TestMethod]
        public void ReturnTrue_When_PasswordIsValid()
        {
            // Arrange
            string password = "password";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Act
            var result = _authManager.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnFalse_When_PasswordIsInvalid()
        {
            // Arrange
            string password = "password";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("differentPassword");

            // Act
            var result = _authManager.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
