using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;
using Telerik_ForumTeamProject.Exceptions;

namespace Telerik_ForumTeamProject.Test.Services.PostServiceTests
{
    [TestClass]
    public class GetPagedPosts_Should
    {
        private Mock<IPostRepository> _mockRepository;
        private PostService _sut;

        [TestInitialize]
        public void Setup()
        {
            var mockExamples = new MockPostRepository();
            this._mockRepository = mockExamples.GetMockRepository();
            this._sut = new PostService(this._mockRepository.Object);
        }

        [TestMethod]
        public void ReturnPagedPosts_When_ValidPage()
        {
            var filterParams = new PostQueryParamteres
            {
                Title = "First Post"
            };

            var result = this._sut.GetPagedPosts(1, 1, filterParams);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(2, result.Metadata.TotalCount);
            Assert.AreEqual(1, result.Metadata.PageSize);
            Assert.AreEqual(2, result.Metadata.TotalPages);
        }

        [TestMethod]
        public void ThrowException_When_PageNotFound()
        {
            var filterParams = new PostQueryParamteres
            {
                Title = "Post"
            };

            Assert.ThrowsException<PageNotFoundException>(() => this._sut.GetPagedPosts(3, 1, filterParams));
        }
    }
}
