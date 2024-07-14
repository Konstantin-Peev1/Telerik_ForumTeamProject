using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;  // Add this line
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;

namespace Telerik_ForumTeamProject.ViewControllers
{
    public class ViewPostController : Controller
    {
        private readonly IPostService postService;
        private readonly ModelMapper modelMapper;
        private readonly ILogger<ViewPostController> logger;  // Add this line

        public ViewPostController(IPostService postService, ModelMapper modelMapper, ILogger<ViewPostController> logger)  // Update constructor
        {
            this.postService = postService;
            this.modelMapper = modelMapper;
            this.logger = logger;  // Add this line
        }

        [HttpGet("Post/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            logger.LogInformation($"Fetching post with ID: {id}");  // Add this line
            var post = postService.GetPost(id);
            if (post == null)
            {
                logger.LogWarning($"Post with ID: {id} not found");  // Add this line
                return NotFound();
            }

            var postToShow = modelMapper.Map(post);
            logger.LogInformation($"Returning view for post with ID: {id}");  // Add this line
            return View("~/Views/Post/Details.cshtml", postToShow);
        }
    }
}
