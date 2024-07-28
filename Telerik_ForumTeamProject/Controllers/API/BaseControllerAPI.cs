using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Telerik_ForumTeamProject.Exceptions;
using Telerik_ForumTeamProject.Helpers;
using Telerik_ForumTeamProject.Models.Entities;

public class BaseControllerAPI : ControllerBase
{
    protected readonly AuthManager authManager;

    public BaseControllerAPI(AuthManager authManager)
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
            return authManager.TryGetUserByUserName(userName);
        }
        throw new EntityNotFoundException("No such user");
    }
}