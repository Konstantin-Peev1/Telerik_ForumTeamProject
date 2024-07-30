using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.IIS.Core;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;
using Telerik_ForumTeamProject.Services;
using Microsoft.Extensions.Configuration;

namespace Telerik_ForumTeamProject.Test.Services.UserServiceTests
{
    [TestClass]
    public class GetUserByInformation_Should
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

        public void ReturnCorrectUser_When_ValidParameterEmail()
        {
            var expectedUser = this._mockRepository.Object.GetByInformation("user1@example.com");

            var actualUser = this._sut.GetByInformation("user1@example.com");

            Assert.AreEqual(expectedUser, actualUser);
        }

        [TestMethod]

        public void ReturnCorrectUser_When_ValidParameterFirstName()
        {
            var expectedUser = this._mockRepository.Object.GetByInformation("string");

            var actualUser = this._sut.GetByInformation("string");

            Assert.AreEqual(expectedUser, actualUser);
        }

        [TestMethod]
        public void ThrowException_When_UserNotFound()
        {

            Assert.ThrowsException<EntityNotFoundException>(() => this._sut.GetByInformationUsername("NonExistentUser"));
        }
    }
}
