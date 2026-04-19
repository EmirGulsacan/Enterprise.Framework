using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace Keycloak.Identity.Shared.Services;

public class KeycloakClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var clone = principal.Clone();
        var newIdentity = (ClaimsIdentity?)clone.Identity;

        if (newIdentity == null)
        {
            return Task.FromResult(principal);
        }

        var realmAccessClaim = principal.FindFirst("realm_access");
        if (realmAccessClaim != null && !string.IsNullOrWhiteSpace(realmAccessClaim.Value))
        {
            try
            {
                using var jsonDocument = JsonDocument.Parse(realmAccessClaim.Value);
                if (jsonDocument.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        var roleName = role.GetString();
                        if (!string.IsNullOrWhiteSpace(roleName))
                        {
                            newIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));
                            newIdentity.AddClaim(new Claim("Permission", roleName));
                        }
                    }
                }
            }
            catch { }
        }

        return Task.FromResult(clone);
    }
}
