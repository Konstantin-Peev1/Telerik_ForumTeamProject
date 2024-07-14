using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Services.Contracts;
using Telerik_ForumTeamProject.ViewModels;

namespace Telerik_ForumTeamProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService _postService;
        private readonly ModelMapper _modelMapper;

        public HomeController(IPostService postService, ModelMapper modelMapper)
        {
            _postService = postService;
            _modelMapper = modelMapper;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var latestPosts = _postService.GetTop10Recent().ToList();
            var mostCommentedPosts = _postService.GetTop10Commented().ToList();

            var viewModel = new HomeViewModel
            {
                LatestPosts = latestPosts.Select(post => _modelMapper.Map(post)).ToList(),
                MostCommentedPosts = mostCommentedPosts.Select(post => _modelMapper.Map(post)).ToList()
            };

            return View(viewModel);
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
    }
}
