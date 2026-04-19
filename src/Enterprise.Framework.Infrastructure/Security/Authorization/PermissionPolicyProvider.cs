namespace Enterprise.Framework.Infrastructure.Security.Authorization;

using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Options;

using System.Threading.Tasks;



public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider {

public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) {
    
override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName) {
    
var policy = await base.GetPolicyAsync(policyName);

        if (policy == null) {
        
policy = new AuthorizationPolicyBuilder() {
                .AddRequirements(new PermissionRequirement(policyName)) {
                .Build();

        }

        return policy;

    }

}


}






}
}



