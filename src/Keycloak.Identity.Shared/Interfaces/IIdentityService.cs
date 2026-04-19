namespace Keycloak.Identity.Shared.Interfaces;
using Keycloak.Identity.Shared.Models;

public interface IIdentityService {

Task<IdentityResult> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<IdentityResult> DeleteUserAsync(string externalId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetUserRolesAsync(string externalId, CancellationToken cancellationToken = default);
}



