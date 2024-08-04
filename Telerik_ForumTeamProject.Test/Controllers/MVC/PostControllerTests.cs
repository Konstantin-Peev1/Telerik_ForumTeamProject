using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Telerik_ForumTeamProject.Controllers.MVC;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.RequestDTO;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Models.ViewModels;
using Telerik_ForumTeamProject.Repositories.Contracts;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Tests.Controllers.MVC
{
    [TestClass]
    public class PostControllerTests
    {
        private Mock<IPostService> _postServiceMock;
        private Mock<ICommentService> _commentServiceMock;
        private Mock<ITagService> _tagServiceMock;
        private Mock<ModelMapper> _modelMapperMock;
        private Mock<ILikeService> _likeServiceMock;
        private Mock<AuthManager> _authManagerMock;
        private PostController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _postServiceMock = new Mock<IPostService>();
            _commentServiceMock = new Mock<ICommentService>();
            _tagServiceMock = new Mock<ITagService>();
            _modelMapperMock = new Mock<ModelMapper>();
            _likeServiceMock = new Mock<ILikeService>();
            var userRepositoryMock = new Mock<IUserRepository>();
            var configurationMock = new Mock<IConfiguration>();

            _authManagerMock = new Mock<AuthManager>(userRepositoryMock.Object, configurationMock.Object);
            _controller = new PostController(
                _postServiceMock.Object,
                _commentServiceMock.Object,
                _tagServiceMock.Object,
                _modelMapperMock.Object,
                _authManagerMock.Object,
                _likeServiceMock.Object
            );

            // Setup HttpContext with ClaimsPrincipal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "John")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            _controller.ControllerContext = controllerContext;

            // Setup TempData
            var tempData = new TempDataDictionary(controllerContext.HttpContext, Mock.Of<ITempDataProvider>())
            {
                ["Success"] = null,
                ["Error"] = null,
                ["ReplyError"] = null,
                ["ReplySuccess"] = null
            };
            _controller.TempData = tempData;

            // Setup mock methods for getting the current user
            userRepositoryMock.Setup(x => x.GetByInformationUsername(It.IsAny<string>())).Returns((string username) =>
            {
                return new User
                {
                    UserName = username,
                    ID = 1,
                    FirstName = "Test",
                    LastName = "User",
                    Email = "testuser@example.com",
                    ProfilePictureUrl = "http://example.com/pic.jpg"
                };
            });
        }


        [TestMethod]
        public void ToggleLike_ShouldReturnJsonResult_WhenPostExists()
        {
            // Arrange
            var user = new User { ID = 1, UserName = "John" };
            var post = new Post { Id = 1, UserID = 2, Likes = new List<Like>() };
            _postServiceMock.Setup(x => x.GetPost(post.Id)).Returns(post);
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);

            // Act
            var result = _controller.ToggleLike(post.Id) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            dynamic jsonResult = result.Value;
            Assert.IsTrue(jsonResult.success);
        }

        [TestMethod]
        public void ToggleLike_ShouldReturnJsonResult_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = 1;
            _postServiceMock.Setup(x => x.GetPost(postId)).Returns((Post)null);

            // Act
            var result = _controller.ToggleLike(postId) as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            dynamic jsonResult = result.Value;
            Assert.IsFalse(jsonResult.success);
            Assert.AreEqual("Post not found", jsonResult.message);
        }

        [TestMethod]
        public void Index_ShouldReturnViewResult_WithPagedPosts()
        {
            // Arrange
            var user = new User { ID = 1, UserName = "John" };
            var pagedPosts = new PagedResult<Post>
            {
                Items = new List<Post>
                {
                    new Post
                    {
                        Id = 1,
                        Title = "Post 1",
                        Content = "Content 1",
                        Created = DateTime.Now,
                        User = user,
                        Likes = new List<Like>(),
                        Comments = new List<Comment>(),
                        Tags = new List<Tag>()
                    }
                },
                Metadata = new PaginationMetadata { TotalCount = 1, PageSize = 10, CurrentPage = 1, TotalPages = 1 }
            };
            _postServiceMock.Setup(x => x.GetPagedPosts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<PostQueryParamteres>())).Returns(pagedPosts);
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);

            // Act
            var result = _controller.Index(new PostQueryParamteres(), 1, 10) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as PagedPostViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Posts.Count);
        }

        [TestMethod]
        public void GetPost_ShouldReturnViewResult_WithPostViewModel()
        {
            // Arrange
            var user = new User { ID = 1, UserName = "John" };
            var post = new Post { Id = 1, Title = "Post 1", Content = "Content 1", Created = DateTime.Now, User = user };
            var comments = new List<Comment>
            {
                new Comment { Id = 1, Content = "Comment 1", User = user, Created = DateTime.Now }
            };
            _postServiceMock.Setup(x => x.GetPost(post.Id)).Returns(post);
            _commentServiceMock.Setup(x => x.GetAllPostComments(post.Id)).Returns(comments);
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);

            // Act
            var result = _controller.GetPost(post.Id) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as PostViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(post.Id, model.id);
            Assert.AreEqual(1, model.Comments.Count);
        }

        [TestMethod]
        public void GetReplies_ShouldReturnPartialViewResult_WithRepliesViewModel()
        {
            // Arrange
            var pagedReplies = new PagedResult<Comment>
            {
                Items = new List<Comment>
                {
                    new Comment { Id = 1, Content = "Reply 1", User = new User { UserName = "John" }, Created = DateTime.Now }
                },
                Metadata = new PaginationMetadata { TotalCount = 1, PageSize = 5, CurrentPage = 1, TotalPages = 1 }
            };
            _commentServiceMock.Setup(x => x.GetPagedReplies(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(pagedReplies);

            // Act
            var result = _controller.GetReplies(1, 1, 5) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as PagedRepliesViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Replies.Count);
        }

        [TestMethod]
        public void Create_Get_ShouldReturnViewResult_WithEmptyPostRequestDTO()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(PostRequestDTO));
        }

        [TestMethod]
        public void Create_Post_ShouldRedirectToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var model = new PostRequestDTO
            {
                Title = "New Post",
                Content = "New Content",
                TagDescriptions = new List<string> { "Tag1,Tag2" }
            };
            var user = new User { ID = 1, UserName = "John" };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            var post = new Post { Id = 1, Title = "New Post", Content = "New Content", User = user };
            _modelMapperMock.Setup(x => x.Map(model)).Returns(post);
            _postServiceMock.Setup(x => x.CreatePost(post, user)).Returns(post);

            // Act
            var result = _controller.Create(model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Post", result.ControllerName);
        }

        [TestMethod]
        public void Create_Post_ShouldReturnViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var model = new PostRequestDTO();
            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = _controller.Create(model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [TestMethod]
        public void Edit_Get_ShouldReturnViewResult_WhenPostExists()
        {
            // Arrange
            var post = new Post
            {
                Id = 1,
                Title = "Post 1",
                Content = "Content 1",
                Tags = new List<Tag> { new Tag { Description = "Tag1" } }
            };
            _postServiceMock.Setup(x => x.GetPost(post.Id)).Returns(post);

            // Act
            var result = _controller.Edit(post.Id) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result.Model as PostRequestDTO;
            Assert.IsNotNull(model);
            Assert.AreEqual(post.Title, model.Title);
        }

        [TestMethod]
        public void Edit_Get_ShouldReturnNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = 1;
            _postServiceMock.Setup(x => x.GetPost(postId)).Returns((Post)null);

            // Act
            var result = _controller.Edit(postId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Edit_Post_ShouldRedirectToIndex_WhenModelStateIsValid()
        {
            // Arrange
            var postId = 1;
            var model = new PostRequestDTO
            {
                Title = "Updated Post",
                Content = "Updated Content",
                TagDescriptions = new List<string> { "Tag1,Tag2" }
            };
            var user = new User { ID = 1, UserName = "John" };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            var post = new Post { Id = postId, Title = "Updated Post", Content = "Updated Content", User = user };
            _modelMapperMock.Setup(x => x.Map(model)).Returns(post);

            // Act
            var result = _controller.Edit(postId, model) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public void Edit_Post_ShouldReturnViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var postId = 1;
            var model = new PostRequestDTO();
            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = _controller.Edit(postId, model) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [TestMethod]
        public void DeleteTag_ShouldRedirectToEdit_WhenTagIsDeleted()
        {
            // Arrange
            var postId = 1;
            var tagDescription = "Tag1";
            var user = new User { ID = 1, UserName = "John" };
            var post = new Post { Id = postId, User = user };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            _postServiceMock.Setup(x => x.GetPost(postId)).Returns(post);

            // Act
            var result = _controller.DeleteTag(postId, tagDescription) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Edit", result.ActionName);
            Assert.AreEqual(postId, result.RouteValues["id"]);
        }

        [TestMethod]
        public void Delete_ShouldReturnOkResult_WhenPostIsDeleted()
        {
            // Arrange
            var postId = 1;
            var user = new User { ID = 1, UserName = "John" };
            var post = new Post { Id = postId, User = user };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            _postServiceMock.Setup(x => x.GetPost(postId)).Returns(post);

            // Act
            var result = _controller.Delete(postId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var value = result.Value as dynamic;
           
        }

        [TestMethod]
        public void Delete_ShouldReturnNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var postId = 1;
            _postServiceMock.Setup(x => x.GetPost(postId)).Returns((Post)null);

            // Act
            var result = _controller.Delete(postId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddComment_ShouldRedirectToGetPost_WhenCommentIsValid()
        {
            // Arrange
            var postId = 1;
            var commentRequest = new CommentRequestDTO { Content = "New Comment" };
            var user = new User { ID = 1, UserName = "John" };
            var comment = new Comment { Id = 1, Content = "New Comment", User = user, PostID = postId };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            _modelMapperMock.Setup(x => x.Map(It.IsAny<CommentRequestDTO>(), It.IsAny<int>(), 0)).Returns(comment);

            // Act
            var result = _controller.AddComment(postId, commentRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("GetPost", result.ActionName);
            Assert.AreEqual(postId, result.RouteValues["id"]);
        }

        [TestMethod]
        public void AddComment_ShouldRedirectToGetPost_WhenCommentIsInvalid()
        {
            // Arrange
            var postId = 1;
            var commentRequest = new CommentRequestDTO { Content = "" };

            // Act
            var result = _controller.AddComment(postId, commentRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("GetPost", result.ActionName);
            Assert.AreEqual(postId, result.RouteValues["id"]);
        }

        [TestMethod]
        public void AddReply_ShouldRedirectToGetPost_WhenReplyIsValid()
        {
            // Arrange
            var postId = 1;
            var parentCommentId = 1;
            var replyRequest = new CommentRequestDTO { Content = "New Reply" };
            var user = new User { ID = 1, UserName = "John" };
            var comment = new Comment { Id = 1, Content = "Comment", PostID = postId, User = user };
            var reply = new Comment { Id = 2, Content = "New Reply", User = user };
            _authManagerMock.Setup(x => x.TryGetUserByUserName(It.IsAny<string>())).Returns(user);
            _commentServiceMock.Setup(x => x.GetComment(parentCommentId)).Returns(comment);
            _modelMapperMock.Setup(x => x.Map(It.IsAny<CommentRequestDTO>(), 0, It.IsAny<int>())).Returns(reply);

            // Act
            var result = _controller.AddReply(postId, parentCommentId, replyRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("GetPost", result.ActionName);
            Assert.AreEqual(comment.PostID, result.RouteValues["id"]);
        }

        [TestMethod]
        public void AddReply_ShouldRedirectToGetPost_WhenReplyIsInvalid()
        {
            // Arrange
            var postId = 1;
            var parentCommentId = 1;
            var replyRequest = new CommentRequestDTO { Content = "" };
            var user = new User { ID = 1, UserName = "John" };
            var comment = new Comment { Id = 1, Content = "Comment", PostID = postId, User = user };
            _commentServiceMock.Setup(x => x.GetComment(parentCommentId)).Returns(comment);

            // Act
            var result = _controller.AddReply(postId, parentCommentId, replyRequest) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("GetPost", result.ActionName);
            Assert.AreEqual(comment.PostID, result.RouteValues["id"]);
            Assert.AreEqual("Reply cannot be empty", _controller.TempData["ReplyError"]);
        }
    }
}
