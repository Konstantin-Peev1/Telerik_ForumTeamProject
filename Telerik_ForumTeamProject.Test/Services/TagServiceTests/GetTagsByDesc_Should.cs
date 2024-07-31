using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.TagServiceTests
{
    [TestClass]
    public class GetTagsByDesc_Should
    {
        private Mock<ITagRepository> _mockRepository;
        private TagService _sut;

        [TestInitialize]
        public void Setup()
        {
            var mockExamples = new MockTagRepository();
            this._mockRepository = mockExamples.GetMockRepository();
            this._sut = new TagService(this._mockRepository.Object);
        }

        [TestMethod]
        public void ReturnTags_When_ValidDescription()
        {
            var desc = "Tag";

            var result = this._sut.GetTagsByDesc(desc);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void ReturnEmpty_When_NoMatchingTags()
        {
            var desc = "NonExistentTag";

            var result = this._sut.GetTagsByDesc(desc);

            Assert.AreEqual(0, result.Count);
        }
    }
}
