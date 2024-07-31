using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.PostServiceTests
{
    [TestClass]
    public class FilterBy_Should
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
        public void ReturnFilteredPosts_When_ValidParameters()
        {
            var filterParams = new PostQueryParamteres
            {
                Title = "First",
                MinLikes = 1,
                MaxLikes = 10
            };

            var result = this._sut.FilterBy(filterParams);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("First Post", result.First().Title);
        }

        [TestMethod]
        public void ReturnEmptyList_When_NoMatches()
        {
            var filterParams = new PostQueryParamteres
            {
                Title = "NonExistent",
                MinLikes = 0,
                MaxLikes = 0
            };

            var result = this._sut.FilterBy(filterParams);

            Assert.AreEqual(0, result.Count);
        }
    }
}
