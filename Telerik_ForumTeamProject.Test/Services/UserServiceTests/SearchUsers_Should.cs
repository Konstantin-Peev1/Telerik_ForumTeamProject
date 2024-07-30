using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.UserServiceTests
{
    [TestClass]
    public class SearchUsers_Should
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
        public void ReturnUsers_When_ValidSearchTerm()
        {
            var searchTerm = "Kosio";

            var result = this._sut.SearchUsers(searchTerm);

            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void ReturnEmpty_When_NoUsersMatchSearchTerm()
        {
            var searchTerm = "NonExistent";

            var result = this._sut.SearchUsers(searchTerm);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void ReturnUsers_When_SearchingByEmail()
        {
            var searchTerm = "user1@example.com";

            var result = this._sut.SearchUsers(searchTerm);

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Kosio", result.First().UserName);
        }
    }
}
