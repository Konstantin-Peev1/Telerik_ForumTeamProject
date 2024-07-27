using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Services.Contracts;
using Telerik_ForumTeamProject.Models.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Telerik_ForumTeamProject.Models.Entities;
using System.Security.Claims;
using Telerik_ForumTeamProject.Services;

namespace Telerik_ForumTeamProject.Controllers.MVC
{
    public class HomeController : Controller
    {
        private readonly IPostService postService;
        private readonly IUserService userService;

        public HomeController(IPostService postService, IUserService userService)
        {
            this.postService = postService;
            this.userService = userService;
        }

        public IActionResult Index(string viewOption = "mostCommented")
        {
            
            var model = new HomeViewModel
            {
                IsAuthenticated = User.Identity != null && User.Identity.IsAuthenticated,
                ViewOption = viewOption
            };

            if (model.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId != null)
                {
                    var user = userService.GetByInformation(userId);
/*                    model.ID = user.ID;
                    model.ProfilePictureUrl = user.ProfilePictureUrl;*/
                }
            }

            if (viewOption == "mostCommented")
            {
                model.TopCommentedPosts = postService.GetTop10Commented().ToList();
            }
            else
            {
                model.RecentPosts = postService.GetTop10Recent().ToList();
            }

            return View(model);
        }
    }
}
