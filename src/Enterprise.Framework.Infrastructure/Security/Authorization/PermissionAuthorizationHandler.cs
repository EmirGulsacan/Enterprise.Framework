namespace Enterprise.Framework.Infrastructure.Security.Authorization;

using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;

using System.Threading.Tasks;



public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement> {

protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement) {
    
if (context.User.HasClaim(c => c.Type == "Permission" && c.Value == requirement.Permission)) {
        
context.Succeed(requirement);

        }

        return Task.CompletedTask;

    }
}



