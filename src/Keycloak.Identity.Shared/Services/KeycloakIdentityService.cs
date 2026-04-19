using Microsoft.Extensions.Options;
using Keycloak.Identity.Shared.Configuration;
using Keycloak.AuthServices.Sdk.Admin;
using Keycloak.AuthServices.Sdk.Admin.Models;
using Keycloak.Identity.Shared.Interfaces;
using Keycloak.Identity.Shared.Models;

namespace Keycloak.Identity.Shared.Services;

public class KeycloakIdentityService : IIdentityService
{
    private readonly IKeycloakUserClient _userClient;
    private readonly KeycloakIdentityOptions _options;

    public KeycloakIdentityService(IKeycloakUserClient userClient, IOptions<KeycloakIdentityOptions> options)
    {
        _userClient = userClient;
        _options = options.Value;
    }

    public async Task<IdentityResult> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = new UserRepresentation
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Enabled = true,
                EmailVerified = true,
                Credentials = new List<CredentialRepresentation>
                {
                    new CredentialRepresentation
                    {
                        Type = "password",
                        Value = request.InitialPassword,
                        Temporary = false
                    }
                },
                Attributes = request.Attributes?.ToDictionary(
                    kvp => kvp.Key,
                    kvp => (ICollection<string>)kvp.Value)
                    ?? new Dictionary<string, ICollection<string>>()
            };

            await _userClient.CreateUserAsync(_options.Realm, user, cancellationToken);

            var users = await _userClient.GetUsersAsync(_options.Realm, new Keycloak.AuthServices.Sdk.Admin.Requests.Users.GetUsersRequestParameters 
            { 
                Username = request.Username 
            });

            var createdUser = users.FirstOrDefault(u => u.Username == request.Username);

            return IdentityResult.Success(createdUser?.Id);
        }
        catch (Exception ex)
        {
            return IdentityResult.Failure(new[] { ex.Message });
        }
    }

    public async Task<IdentityResult> DeleteUserAsync(string externalId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _userClient.DeleteUserAsync(_options.Realm, externalId, cancellationToken);
            return IdentityResult.Success();
        }
        catch (Exception ex)
        {
            return IdentityResult.Failure(new[] { ex.Message });
        }
    }

    public async Task<IReadOnlyList<string>> GetUserRolesAsync(string externalId, CancellationToken cancellationToken = default)
    {
        // TODO: Keycloak.AuthServices.Sdk 2.5.0 güncellemesi sonrasý Role Mapping iþlemleri ayrýldý.
        // Ýleride IKeycloakRoleMapperClient (veya güncel interface) inject edilerek burasý doldurulacak.
        // Þu an Phase 2'yi bloklamamasý için geçici olarak boþ liste dönüyoruz. (Yetkiler zaten token'dan geliyor).
        return await Task.FromResult(new List<string>());
    }
}
