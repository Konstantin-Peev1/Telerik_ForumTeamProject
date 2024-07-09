using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;

public class BaseController : ControllerBase
{
    protected readonly AuthManager authManager;

    public BaseController(AuthManager authManager)
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