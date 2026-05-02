namespace Enterprise.Framework.Application.Common.Interfaces;

public interface IKeycloakAdminService
{
    Task<string> CreateUserAsync(string username, string email, string firstName, string lastName, string password, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(string identityId, string email, string firstName, string lastName, bool enabled, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string identityId, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(string identityId, string newPassword, CancellationToken cancellationToken = default);
    Task AssignRolesToUserAsync(string identityId, IEnumerable<string> roleNames, CancellationToken cancellationToken = default);
    Task RemoveRolesFromUserAsync(string identityId, IEnumerable<string> roleNames, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetUserRoleNamesAsync(string identityId, CancellationToken cancellationToken = default);
    Task CreateRoleAsync(string roleName, string? description = null, CancellationToken cancellationToken = default);
    Task UpdateRoleAsync(string oldRoleName, string newRoleName, string? description = null, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetAllRealmRoleNamesAsync(CancellationToken cancellationToken = default);
}
