using Microsoft.AspNetCore.Mvc;

namespace Telerik_ForumTeamProject.ViewControllers
{
    public class UserViewController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
