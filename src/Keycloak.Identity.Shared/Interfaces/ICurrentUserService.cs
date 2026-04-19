namespace Keycloak.Identity.Shared.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Email { get; }
    string? OrganizationId { get; }
    string? BranchId { get; }
    bool IsAdmin { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Permissions { get; }
    Task<bool> HasPermissionAsync(string permission, CancellationToken cancellationToken = default);
}
