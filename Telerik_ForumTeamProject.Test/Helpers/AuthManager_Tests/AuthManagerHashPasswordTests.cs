using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik_ForumTeamProject.Helpers;

namespace Telerik_ForumTeamProject.Test.Helpers.AuthManager_Tests
{
    [TestClass]
    public class HashPassword_Should
    {
        private AuthManager _authManager;

        [TestInitialize]
        public void SetUp()
        {
            _authManager = new AuthManager(null, null);
        }

        [TestMethod]
        public void ReturnHashedPassword_When_PasswordIsValid()
        {
            // Arrange
            string password = "password";

            // Act
            var hashedPassword = _authManager.HashPassword(password);

            // Assert
            Assert.IsNotNull(hashedPassword);
            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(password, hashedPassword));
        }
    }
}
