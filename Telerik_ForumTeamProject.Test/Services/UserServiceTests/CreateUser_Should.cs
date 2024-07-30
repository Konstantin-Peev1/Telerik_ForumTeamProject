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
    public class CreateUser_Should
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
        public void CreateUser_When_ValidUser()
        {
            var newUser = new User
            {
                UserName = "NewUser",
                Email = "newuser@example.com",
                Password = "password",
                ProfilePictureUrl = ""
            };

            var createdUser = this._sut.CreateUser(newUser);

            Assert.IsNotNull(createdUser);
            Assert.AreEqual(newUser.UserName, createdUser.UserName);
            Assert.AreEqual(newUser.Email, createdUser.Email);
            Assert.AreEqual(newUser.Password, createdUser.Password);
            Assert.AreEqual("http://example.com/default-profile-picture.png", createdUser.ProfilePictureUrl);
        }

        [TestMethod]
        public void ThrowException_When_UserWithUsernameExists()
        {
            var newUser = new User
            {
                UserName = "Kosio", // Assuming this username already exists
                Email = "newemail@example.com",
                Password = "password"
            };

            Assert.ThrowsException<DuplicateEntityException>(() => this._sut.CreateUser(newUser));
        }

        [TestMethod]
        public void ThrowException_When_UserWithEmailExists()
        {
            var newUser = new User
            {
                UserName = "NewUser",
                Email = "user1@example.com", // Assuming this email already exists
                Password = "password"
            };

            Assert.ThrowsException<DuplicateEntityException>(() => this._sut.CreateUser(newUser));
        }
    }
}
