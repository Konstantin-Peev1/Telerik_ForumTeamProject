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
    public class UpdateProfilePicture_Should
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
        public void UpdateProfilePicture_When_UserExists()
        {
            var userId = 1;
            var newProfilePictureUrl = "http://example.com/new-profile-picture.png";

            var updatedUser = this._sut.UpdateProfilePicture(userId, newProfilePictureUrl);

            Assert.AreEqual(newProfilePictureUrl, updatedUser.ProfilePictureUrl);
        }

        [TestMethod]
        public void ThrowException_When_UserNotFound()
        {
            var userId = 999;
            var newProfilePictureUrl = "http://example.com/new-profile-picture.png";

            Assert.ThrowsException<EntityNotFoundException>(() => this._sut.UpdateProfilePicture(userId, newProfilePictureUrl));
        }
    }
}
