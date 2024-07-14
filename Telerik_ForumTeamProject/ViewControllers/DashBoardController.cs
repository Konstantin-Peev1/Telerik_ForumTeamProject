using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.ResponseDTO;
using Telerik_ForumTeamProject.Services.Contracts;
using Telerik_ForumTeamProject.ViewModels;

namespace Telerik_ForumTeamProject.Controllers
{
     // Ensure this attribute is present to require authentication
    public class DashboardController : Controller
    {
        private readonly IPostService postService;
        private readonly ModelMapper modelMapper;

        public DashboardController(IPostService postService, ModelMapper modelMapper)
        {
            this.postService = postService;
            this.modelMapper = modelMapper;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var posts = postService.GetAll().ToList();
            var viewModel = posts.Select(x => this.modelMapper.Map(x)).ToList();



            return View("Dashboard");
        }
    }
}
