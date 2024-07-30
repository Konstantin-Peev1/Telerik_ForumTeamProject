using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.UserServiceTests
{
    [TestClass]
    public class UserExists_Should
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
        public void ReturnTrue_When_UserExists()
        {
            var username = "Kosio";

            var exists = this._sut.UserExists(username);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void ReturnFalse_When_UserDoesNotExist()
        {
            var username = "NonExistentUser";

            var exists = this._sut.UserExists(username);

            Assert.IsFalse(exists);
        }
    }
}
