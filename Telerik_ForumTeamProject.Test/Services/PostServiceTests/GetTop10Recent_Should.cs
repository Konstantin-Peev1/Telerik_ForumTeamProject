using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.PostServiceTests
{
    [TestClass]
    public class GetTop10Recent_Should
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
        public void ReturnTop10RecentPosts()
        {
            var result = this._sut.GetTop10Recent();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Second Post", result.First().Title);
        }
    }
}
