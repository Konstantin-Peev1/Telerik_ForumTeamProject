using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;

namespace Telerik_ForumTeamProject.Controllers.MVC
{
    public class BaseControllerMVC : Controller
    {
        private readonly AuthManager authManager;

        public BaseControllerMVC(AuthManager authManager)
        {
            this.authManager = authManager;
        }
        protected User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;
                var userName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;
                var password = userClaims.First(o => o.Type == ClaimTypes.Hash)?.Value;
                string credentials = $"{userName}:{password}";
                return authManager.TryGetUser(credentials);
            }
            throw new EntityNotFoundException("No such user");
        }
    }
}
