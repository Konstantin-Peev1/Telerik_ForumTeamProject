using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Test.Services.TagServiceTests
{
    [TestClass]
    public class Create_Should
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
        public void CreateTag_When_ValidDescription()
        {
            var newTagDesc = "NewTag";

            var createdTag = this._sut.Create(newTagDesc);

            Assert.IsNotNull(createdTag);
            Assert.AreEqual(newTagDesc, createdTag.Description);
        }
    }
}
