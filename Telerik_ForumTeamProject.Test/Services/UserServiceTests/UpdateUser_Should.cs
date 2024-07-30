using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.UserServiceTests
{
    [TestClass]
    public class UpdateUser_Should
    {
        private Mock<IUserRepository> _mockRepository;
        private UserService _sut;
        private IConfiguration _configuration;

        [TestInitialize]
        public void Setup()
        {
            var mockExamples = new MockUserRepository();
            this._mockRepository = mockExamples.GetMockRepository();

            var inMemorySettings = new Dictionary<string, string> {
                {"CloudinarySettings:DefaultProfilePictureUrl", "http://example.com/default-profile-picture.png"},
            };

            this._configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            this._sut = new UserService(this._mockRepository.Object, this._configuration);
        }

        [TestMethod]
        public void UpdateUser_When_ValidUser()
        {
            var existingUser = this._mockRepository.Object.GetUserByID(1);
            var updatedUser = new User
            {
                ID = 1,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Email = "updateduser@example.com",
                Password = "newpassword"
            };

            var result = this._sut.UpdateUser(existingUser, updatedUser, 1);

            Assert.AreEqual(updatedUser.FirstName, result.FirstName);
            Assert.AreEqual(updatedUser.LastName, result.LastName);
            Assert.AreEqual(updatedUser.Email, result.Email);
            Assert.AreEqual(updatedUser.Password, result.Password);
        }

        [TestMethod]
        public void ThrowException_When_UpdatingOtherUser()
        {
            var existingUser = this._mockRepository.Object.GetUserByID(1);
            var updatedUser = new User
            {
                ID = 2,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Email = "updateduser@example.com",
                Password = "newpassword"
            };

            Assert.ThrowsException<AuthorisationExcpetion>(() => this._sut.UpdateUser(this._mockRepository.Object.GetUserByID(2), updatedUser, 1));
        }

        [TestMethod]
        public void ThrowException_When_UsernameExists()
        {
            var existingUser = this._mockRepository.Object.GetUserByID(1);
            var updatedUser = new User
            {
                ID = 1,
                UserName = "Kosio1", // Assuming this username already exists
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Email = "updateduser@example.com",
                Password = "newpassword"
            };

            Assert.ThrowsException<DuplicateEntityException>(() => this._sut.UpdateUser(existingUser, updatedUser, 1));
        }
    }
}
