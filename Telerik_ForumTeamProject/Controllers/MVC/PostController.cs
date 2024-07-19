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
        public IActionResult Index(int id)
        {
            id = 2;
            Post post = _postService.GetPost(id);
            var comments = _commentService.GetAllPostComments(id);

            PostViewModel viewModel = new PostViewModel
            {
                Title = post.Title,
                Content = post.Content,
                Likes = post.Likes?.Count ?? 0,
                PostDate = post.Created.ToString("yyyy-MM-dd HH:mm:ss"), // Assuming Created is the date property
                Comments = comments?.Select(comment => new CommentReplyResponseDTO
                {
                    Content = comment?.Content ?? "No content",
                    // Map other properties as needed and handle null checks
                }).ToList() ?? new List<CommentReplyResponseDTO>(),
                UserName = post.User.UserName,
                Tags = post.Tags?.Select(tag => tag.Description).ToList() ?? new List<string>()
            };
            return View(viewModel);
        }

/*        [HttpGet]
        public IActionResult GetPost(int id)
        {

        }*/
    }
}
