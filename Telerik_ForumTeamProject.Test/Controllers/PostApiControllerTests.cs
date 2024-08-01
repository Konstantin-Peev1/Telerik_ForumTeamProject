using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik_ForumTeamProject.Controllers;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Telerik_ForumTeamProject.Repositories.Contracts;

namespace Telerik_ForumTeamProject.Tests.Controllers.PostControllerAPITests
{
    [TestClass]
    public class PostControllerAPITests
    {
        private Mock<IPostService> _postServiceMock;
        private Mock<ModelMapper> _modelMapperMock;
        private Mock<AuthManager> _authManagerMock;
        private PostControllerAPI _controller;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IUserRepository> _userRepositoryMock;

        [TestInitialize]
        public void SetUp()
        {
            this._postServiceMock = new Mock<IPostService>();
            this._modelMapperMock = new Mock<ModelMapper>();
            this._httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            this._configurationMock = new Mock<IConfiguration>();
            this._userRepositoryMock = new Mock<IUserRepository>();

            this._authManagerMock = new Mock<AuthManager>(this._userRepositoryMock.Object, this._configurationMock.Object);

            this._controller = new PostControllerAPI(this._postServiceMock.Object, this._modelMapperMock.Object, this._authManagerMock.Object);

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
        public void ReturnLatest10Posts_When_Called()
        {
            // Arrange
            List<Post> posts = new List<Post> { new Post { Id = 1, Title = "Test Post", Content = "Content", Likes = new List<Like>(), Created = DateTime.UtcNow, User = new User { UserName = "User1" }, Comments = new List<Comment>(), Tags = new List<Tag> { new Tag { Description = "Tag1" } } } };
            List<PostUploadResponseDTO> postDtos = new List<PostUploadResponseDTO> { new PostUploadResponseDTO { Title = "Test Post", Content = "Content", Likes = 0, PostDate = DateTime.UtcNow.ToString(), UserName = "User1", Comments = new List<CommentReplyResponseDTO>(), Tags = new List<string> { "Tag1" } } };

            this._postServiceMock.Setup(x => x.GetTop10Recent()).Returns(posts);
            this._modelMapperMock.Setup(x => x.Map(It.IsAny<Post>())).Returns((Post post) => new PostUploadResponseDTO { Title = post.Title, Content = post.Content, Likes = post.Likes.Count, PostDate = post.Created.ToString(), UserName = post.User.UserName, Comments = new List<CommentReplyResponseDTO>(), Tags = post.Tags.Select(t => t.Description).ToList() });

            // Act
            var result = this._controller.GetLatest10();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(postDtos.Count, ((List<PostUploadResponseDTO>)okResult.Value).Count);
        }

        [TestMethod]
        public void ReturnMostCommented10Posts_When_Called()
        {
            // Arrange
            List<Post> posts = new List<Post> { new Post { Id = 1, Title = "Test Post", Content = "Content", Likes = new List<Like>(), Created = DateTime.UtcNow, User = new User { UserName = "User1" }, Comments = new List<Comment>(), Tags = new List<Tag> { new Tag { Description = "Tag1" } } } };
            List<PostUploadResponseDTO> postDtos = new List<PostUploadResponseDTO> { new PostUploadResponseDTO { Title = "Test Post", Content = "Content", Likes = 0, PostDate = DateTime.UtcNow.ToString(), UserName = "User1", Comments = new List<CommentReplyResponseDTO>(), Tags = new List<string> { "Tag1" } } };

            this._postServiceMock.Setup(x => x.GetTop10Commented()).Returns(posts);
            this._modelMapperMock.Setup(x => x.Map(It.IsAny<Post>())).Returns((Post post) => new PostUploadResponseDTO { Title = post.Title, Content = post.Content, Likes = post.Likes.Count, PostDate = post.Created.ToString(), UserName = post.User.UserName, Comments = new List<CommentReplyResponseDTO>(), Tags = post.Tags.Select(t => t.Description).ToList() });

            // Act
            var result = this._controller.GetMostCommented10();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(postDtos.Count, ((List<PostUploadResponseDTO>)okResult.Value).Count);
        }

        [TestMethod]
        public void ReturnFilteredPosts_When_ValidParameters()
        {
            // Arrange
            PostQueryParamteres parameters = new PostQueryParamteres { Title = "Test" };
            List<Post> posts = new List<Post> { new Post { Id = 1, Title = "Test Post", Content = "Content", Likes = new List<Like>(), Created = DateTime.UtcNow, User = new User { UserName = "User1" }, Comments = new List<Comment>(), Tags = new List<Tag> { new Tag { Description = "Tag1" } } } };
            List<PostUploadResponseDTO> postDtos = new List<PostUploadResponseDTO> { new PostUploadResponseDTO { Title = "Test Post", Content = "Content", Likes = 0, PostDate = DateTime.UtcNow.ToString(), UserName = "User1", Comments = new List<CommentReplyResponseDTO>(), Tags = new List<string> { "Tag1" } } };

            this._postServiceMock.Setup(x => x.FilterBy(parameters)).Returns(posts);
            this._modelMapperMock.Setup(x => x.Map(It.IsAny<Post>())).Returns((Post post) => new PostUploadResponseDTO { Title = post.Title, Content = post.Content, Likes = post.Likes.Count, PostDate = post.Created.ToString(), UserName = post.User.UserName, Comments = new List<CommentReplyResponseDTO>(), Tags = post.Tags.Select(t => t.Description).ToList() });

            // Act
            var result = this._controller.Get(parameters);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(postDtos.Count, ((List<PostUploadResponseDTO>)okResult.Value).Count);
        }

        [TestMethod]
        public void ReturnPostById_When_ValidId()
        {
            // Arrange
            int postId = 1;
            Post post = new Post { Id = postId, Title = "Test Post", Content = "Content", Likes = new List<Like>(), Created = DateTime.UtcNow, User = new User { UserName = "User1" }, Comments = new List<Comment>(), Tags = new List<Tag> { new Tag { Description = "Tag1" } } };
            PostUploadResponseDTO postDto = new PostUploadResponseDTO { Title = "Test Post", Content = "Content", Likes = 0, PostDate = DateTime.UtcNow.ToString(), UserName = "User1", Comments = new List<CommentReplyResponseDTO>(), Tags = new List<string> { "Tag1" } };

            this._postServiceMock.Setup(x => x.GetPost(postId)).Returns(post);
            this._modelMapperMock.Setup(x => x.Map(post)).Returns(postDto);

            // Act
            var result = this._controller.GetById(postId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(postDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnCreatedPost_When_ValidPost()
        {
            // Arrange
            PostRequestDTO postRequestDto = new PostRequestDTO { Title = "New Post", Content = "Content" };
            Post post = new Post { Id = 1, Title = "New Post", Content = "Content", Likes = new List<Like>(), Created = DateTime.UtcNow, User = new User { UserName = "User1" }, Comments = new List<Comment>(), Tags = new List<Tag> { new Tag { Description = "Tag1" } } };
            PostUploadResponseDTO postDto = new PostUploadResponseDTO { Title = "New Post", Content = "Content", Likes = 0, PostDate = DateTime.UtcNow.ToString(), UserName = "User1", Comments = new List<CommentReplyResponseDTO>(), Tags = new List<string> { "Tag1" } };

            this._modelMapperMock.Setup(x => x.Map(postRequestDto)).Returns(post);
            this._postServiceMock.Setup(x => x.CreatePost(post, It.IsAny<User>())).Returns(post);
            this._modelMapperMock.Setup(x => x.Map(post)).Returns(postDto);

            // Act
            var result = this._controller.CreatePost(postRequestDto);

            // Assert
            var createdResult = result as ObjectResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);
            Assert.AreEqual(postDto, createdResult.Value);
        }

        [TestMethod]
        public void ReturnUpdatedPost_When_ValidPost()
        {
            // Arrange
            int postId = 1;
            PostRequestDTO postRequestDto = new PostRequestDTO { Title = "Updated Post", Content = "Content" };
            Post post = new Post { Id = postId, Title = "Updated Post", Content = "Content", Likes = new List<Like>(), Created = DateTime.UtcNow, User = new User { UserName = "User1" }, Comments = new List<Comment>(), Tags = new List<Tag> { new Tag { Description = "Tag1" } } };
            PostUploadResponseDTO postDto = new PostUploadResponseDTO { Title = "Updated Post", Content = "Content", Likes = 0, PostDate = DateTime.UtcNow.ToString(), UserName = "User1", Comments = new List<CommentReplyResponseDTO>(), Tags = new List<string> { "Tag1" } };

            this._modelMapperMock.Setup(x => x.Map(postRequestDto)).Returns(post);
            this._postServiceMock.Setup(x => x.UpdatePost(postId, post, It.IsAny<User>())).Returns(post);
            this._modelMapperMock.Setup(x => x.Map(post)).Returns(postDto);

            // Act
            var result = this._controller.UpdatePost(postRequestDto, postId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(postDto, okResult.Value);
        }

        [TestMethod]
        public void ReturnOk_When_DeletingValidPost()
        {
            // Arrange
            int postId = 1;

            this._postServiceMock.Setup(x => x.DeletePost(postId, It.IsAny<User>())).Returns(true);

            // Act
            var result = this._controller.DeletePost(postId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(true, okResult.Value);
        }

        [TestMethod]
        public void ReturnPagedPosts_When_ValidPage()
        {
            // Arrange
            PostQueryParamteres parameters = new PostQueryParamteres { Title = "Test" };
            PagedResult<Post> pagedPosts = new PagedResult<Post>
            {
                Items = new List<Post> { new Post { Id = 1, Title = "Test Post", Content = "Content", Likes = new List<Like>(), Created = DateTime.UtcNow, User = new User { UserName = "User1" }, Comments = new List<Comment>(), Tags = new List<Tag> { new Tag { Description = "Tag1" } } } },
                Metadata = new PaginationMetadata { TotalCount = 1, PageSize = 10, CurrentPage = 1, TotalPages = 1 }
            };
            PagedResult<PostUploadResponseDTO> pagedPostDtos = new PagedResult<PostUploadResponseDTO>
            {
                Items = new List<PostUploadResponseDTO> { new PostUploadResponseDTO { Title = "Test Post", Content = "Content", Likes = 0, PostDate = DateTime.UtcNow.ToString(), UserName = "User1", Comments = new List<CommentReplyResponseDTO>(), Tags = new List<string> { "Tag1" } } },
                Metadata = pagedPosts.Metadata
            };

            this._postServiceMock.Setup(x => x.GetPagedPosts(1, 10, parameters)).Returns(pagedPosts);
            this._modelMapperMock.Setup(x => x.Map(It.IsAny<Post>())).Returns((Post post) => new PostUploadResponseDTO { Title = post.Title, Content = post.Content, Likes = post.Likes.Count, PostDate = post.Created.ToString(), UserName = post.User.UserName, Comments = new List<CommentReplyResponseDTO>(), Tags = post.Tags.Select(t => t.Description).ToList() });

            // Act
            var result = this._controller.ShowAllPosts(parameters);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var response = okResult.Value as PagedResult<PostUploadResponseDTO>;
            Assert.IsNotNull(response);
            Assert.AreEqual(pagedPostDtos.Items.Count, response.Items.Count);
            Assert.AreEqual(pagedPostDtos.Metadata.TotalCount, response.Metadata.TotalCount);
        }
    }
}
