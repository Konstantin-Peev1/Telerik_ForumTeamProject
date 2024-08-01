using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Telerik_ForumTeamProject.Controllers;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Tests.Controllers.TagControllerAPITests
{
    [TestClass]
    public class TagControllerAPITests
    {
        private Mock<ITagService> _tagServiceMock;
        private Mock<ModelMapper> _modelMapperMock;
        private Mock<AuthManager> _authManagerMock;
        private Mock<IPostService> _postServiceMock;
        private TagControllerAPI _controller;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IUserRepository> _userRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            this._tagServiceMock = new Mock<ITagService>();
            this._modelMapperMock = new Mock<ModelMapper>();
            this._postServiceMock = new Mock<IPostService>();
            this._httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            this._configurationMock = new Mock<IConfiguration>();
            this._userRepositoryMock = new Mock<IUserRepository>();

            this._authManagerMock = new Mock<AuthManager>(this._userRepositoryMock.Object, this._configurationMock.Object);

            this._controller = new TagControllerAPI(this._tagServiceMock.Object, this._modelMapperMock.Object, this._authManagerMock.Object, this._postServiceMock.Object);

            // Mock the HttpContext and set up a ClaimsPrincipal
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "User1"),
            }, "mock"));

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(user);

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            this._controller.ControllerContext = controllerContext;
        }

        [TestMethod]
        public void ReturnOk_When_GettingTagsValid()
        {
            // Arrange
            string desc = "tag";
            List<Tag> tags = new List<Tag> { new Tag { ID = 1, Description = "tag" } };
            List<TagResponseDTO> tagsDto = new List<TagResponseDTO> { new TagResponseDTO { Description = "tag" } };

            this._tagServiceMock.Setup(x => x.GetTagsByDesc(desc)).Returns(tags);
            this._modelMapperMock.Setup(x => x.Map(It.IsAny<Tag>())).Returns(tagsDto.First());

            // Act
            var result = this._controller.Get(desc);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            
        }

        [TestMethod]
        public void ReturnOk_When_UpdatingTagsValid()
        {
            // Arrange
            int postId = 1;
            string desc = "new tag";
            User user = new User { ID = 1, UserName = "User1" };
            Post post = new Post { Id = postId, UserID = user.ID };
            Tag tag = new Tag { ID = 1, Description = desc };
            PostUploadResponseDTO postDto = new PostUploadResponseDTO { Title = "Post", Content = "Content", Tags = new List<string> { desc } };

            this._authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            this._postServiceMock.Setup(x => x.GetPost(postId)).Returns(post);
            this._tagServiceMock.Setup(x => x.UpdateTags(user, post, desc)).Returns(tag);
            this._modelMapperMock.Setup(x => x.Map(post)).Returns(postDto);

            // Act
            var result = this._controller.UpdateTags(postId, desc);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(postDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnOk_When_DeletingTagValid()
        {
            // Arrange
            int postId = 1;
            string desc = "tag";
            User user = new User { ID = 1, UserName = "User1" };
            Post post = new Post { Id = postId, UserID = user.ID };
            bool deletedTag = true;

            this._authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            this._postServiceMock.Setup(x => x.GetPost(postId)).Returns(post);
            this._tagServiceMock.Setup(x => x.RemoveTags(user, post, desc)).Returns(deletedTag);

            // Act
            var result = this._controller.Delete(postId, desc);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(deletedTag, okResult.Value);
        }
    }
}
