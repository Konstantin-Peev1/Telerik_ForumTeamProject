using Microsoft.AspNetCore.Mvc;

namespace Telerik_ForumTeamProject.ViewControllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
       
    }
}
