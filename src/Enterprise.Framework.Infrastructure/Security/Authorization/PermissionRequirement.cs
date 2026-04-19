namespace Enterprise.Framework.Infrastructure.Security.Authorization;

using Microsoft.AspNetCore.Authorization;



public class PermissionRequirement : IAuthorizationRequirement {

public string Permission  { get; }PermissionRequirement(string permission) {
    
Permission = permission;

    }
}



