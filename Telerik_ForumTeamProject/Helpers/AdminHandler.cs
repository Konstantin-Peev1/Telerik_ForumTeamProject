using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

public class AdminRequirement : IAuthorizationRequirement { }

public class AdminHandler : AuthorizationHandler<AdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
    {
        var userClaims = context.User.Claims;
        var isAdminClaim = userClaims.FirstOrDefault(c => c.Type == "isAdmin")?.Value;

        if (isAdminClaim != null && bool.TryParse(isAdminClaim, out bool isAdmin) && isAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}