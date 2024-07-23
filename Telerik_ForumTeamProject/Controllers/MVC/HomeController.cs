using Microsoft.AspNetCore.Mvc;
using Telerik_ForumTeamProject.Services.Contracts;
using Telerik_ForumTeamProject.Models.ViewModels;
using System.Linq;

namespace Telerik_ForumTeamProject.Controllers.MVC
{
    public class HomeController : Controller
    {
        private readonly IPostService postService;

        public HomeController(IPostService postService)
        {
            this.postService = postService;
        }

        public IActionResult Index(string viewOption = "mostCommented")
        {
            var model = new HomeViewModel
            {
                IsAuthenticated = User.Identity != null && User.Identity.IsAuthenticated,
                ViewOption = viewOption
            };

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
