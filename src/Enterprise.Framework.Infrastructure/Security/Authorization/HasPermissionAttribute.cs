namespace Enterprise.Framework.Infrastructure.Security.Authorization;

using Microsoft.AspNetCore.Authorization;



public class HasPermissionAttribute : AuthorizeAttribute {

public HasPermissionAttribute(string permission) : base(permission) {
    
}
}



