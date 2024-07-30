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
    public class UnblockUser_Should
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
        public void UnblockUser_When_UserIsBlocked()
        {
            var userToUnblock = this._mockRepository.Object.GetUserByID(2);
            userToUnblock.IsBlocked = true;

            var result = this._sut.UnBlockUser(userToUnblock);

            Assert.IsFalse(result.IsBlocked);
        }

        [TestMethod]
        public void ThrowException_When_UserIsNotBlocked()
        {
            var userToUnblock = this._mockRepository.Object.GetUserByID(2);

            Assert.ThrowsException<DuplicateEntityException>(() => this._sut.UnBlockUser(userToUnblock));
        }
    }
}
