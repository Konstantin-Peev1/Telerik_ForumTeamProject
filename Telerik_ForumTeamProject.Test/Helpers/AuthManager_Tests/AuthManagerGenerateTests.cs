using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Tests.AuthManagerTests
{
    [TestClass]
    public class Generate_Should
    {
        private Mock<IConfiguration> _configurationMock;
        private AuthManager _authManager;

        [TestInitialize]
        public void SetUp()
        {
            this._configurationMock = new Mock<IConfiguration>();
            this._configurationMock.Setup(c => c["Jwt:Key"]).Returns("verySecretKey12345678901234567890");
            this._configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            this._configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

            this._authManager = new AuthManager(null, this._configurationMock.Object);
        }

        [TestMethod]
        public void ReturnToken_When_UserIsValid()
        {
            // Arrange
            var user = new User
            {
                UserName = "testUser",
                Email = "test@example.com",
                FirstName = "First",
                LastName = "Last",
                IsAdmin = false,
                ProfilePictureUrl = "http://example.com/pic.jpg"
            };

            // Act
            var token = this._authManager.Generate(user);

            // Assert
            Assert.IsNotNull(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("verySecretKey12345678901234567890"));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "TestIssuer",
                ValidateAudience = true,
                ValidAudience = "TestAudience",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            Assert.IsNotNull(validatedToken);
            Assert.AreEqual("testUser", principal.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}
