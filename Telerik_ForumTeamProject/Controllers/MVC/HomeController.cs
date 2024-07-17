using Microsoft.AspNetCore.Mvc;

namespace Telerik_ForumTeamProject.Controllers.MVC
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
