using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Telerik_ForumTeamProject.Models.ServiceModel;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Tests.Services
{
    [TestClass]
    public class CloudinaryServiceTests
    {
        private Mock<Cloudinary> _cloudinaryMock;
        private CloudinaryService _cloudinaryService;

        [TestInitialize]
        public void SetUp()
        {
            _cloudinaryMock = new Mock<Cloudinary>(new Account("cloudName", "apiKey", "apiSecret"));
            _cloudinaryService = new CloudinaryService(_cloudinaryMock.Object);
        }

    

        [TestMethod]
        public async Task UploadImageAsync_ShouldReturnNull_WhenFileIsEmpty()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.Length).Returns(0);

            // Act
            var result = await _cloudinaryService.UploadImageAsync(fileMock.Object);

            // Assert
            Assert.IsNull(result);
        }
    }
}
