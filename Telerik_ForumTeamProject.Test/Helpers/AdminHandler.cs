using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YourNamespace.Tests
{
    [TestClass]
    public class AdminHandlerTests
    {
        private AdminHandler _adminHandler;
        private AdminRequirement _requirement;

        [TestInitialize]
        public void SetUp()
        {
            _adminHandler = new AdminHandler();
            _requirement = new AdminRequirement();
        }

        [TestMethod]
        public async Task HandleRequirementAsync_ShouldSucceed_WhenUserIsAdmin()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testuser"),
                new Claim(ClaimTypes.Email, "testuser@example.com"),
                new Claim(ClaimTypes.GivenName, "Test"),
                new Claim(ClaimTypes.Surname, "User"),
                new Claim("isAdmin", "True")
            }, "mock"));

            var context = new AuthorizationHandlerContext(new[] { _requirement }, user, null);

            // Act
            await _adminHandler.HandleAsync(context);

            // Assert
            Assert.IsTrue(context.HasSucceeded);
        }

        [TestMethod]
        public async Task HandleRequirementAsync_ShouldNotSucceed_WhenUserIsNotAdmin()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testuser"),
                new Claim(ClaimTypes.Email, "testuser@example.com"),
                new Claim(ClaimTypes.GivenName, "Test"),
                new Claim(ClaimTypes.Surname, "User"),
                new Claim("isAdmin", "False")
            }, "mock"));

            var context = new AuthorizationHandlerContext(new[] { _requirement }, user, null);

            // Act
            await _adminHandler.HandleAsync(context);

            // Assert
            Assert.IsFalse(context.HasSucceeded);
        }
    }
}
