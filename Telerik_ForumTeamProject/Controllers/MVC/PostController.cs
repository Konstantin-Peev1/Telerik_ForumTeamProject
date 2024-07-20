using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Xml.Linq;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Models.ViewModels;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.Controllers.MVC
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly ModelMapper _modelMapper;
        public PostController(IPostService postService, ICommentService commentService, ModelMapper modelMapper)
        {
            _postService = postService;
            _commentService = commentService;
            _modelMapper = modelMapper;
        }
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            PagedResult<Post> pagedPosts = _postService.GetPagedPosts(page, pageSize);

            var postViewModels = pagedPosts.Items.Select(post => new PostViewModel
            {
                Title = post.Title,
                Content = post.Content,
                Likes = post.Likes?.Count ?? 0,
                PostDate = post.Created.ToString("yyyy-MM-dd HH:mm:ss"),
                Comments = post.Comments?.Select(comment => new CommentReplyResponseDTO
                {
                    Content = comment?.Content ?? "No content",
                    // Map other properties as needed
                }).ToList() ?? new List<CommentReplyResponseDTO>(),
                UserName = post.User.UserName,
                Tags = post.Tags?.Select(tag => tag.Description).ToList() ?? new List<string>()
            }).ToList();

            var pagedPostViewModel = new PagedPostViewModel
            {
                Posts = postViewModels,
                PaginationMetadata = pagedPosts.Metadata
            };

            return View(pagedPostViewModel);
        }

        [HttpGet]
        public IActionResult GetPost(int id)
        {
            id = 2;
            Post post = _postService.GetPost(id);
            var comments = _commentService.GetAllPostComments(id);

            PostViewModel viewModel = new PostViewModel
            {
                Title = post.Title,
                Content = post.Content,
                Likes = post.Likes?.Count ?? 0,
                PostDate = post.Created.ToString("yyyy-MM-dd HH:mm:ss"),
                Comments = comments?.Select(comment => new CommentReplyResponseDTO
                {
                    Content = comment?.Content ?? "No content",
                    
                }).ToList() ?? new List<CommentReplyResponseDTO>(),
                UserName = post.User.UserName,
                Tags = post.Tags?.Select(tag => tag.Description).ToList() ?? new List<string>()
            };
            return RedirectToAction("Post", viewModel);
        }
    }
}
