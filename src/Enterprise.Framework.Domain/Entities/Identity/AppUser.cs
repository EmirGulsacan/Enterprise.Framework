namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

public class AppUser : BaseEntity, IOrganizationBoundEntity
{
    public string IdentityId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? OrganizationId { get; set; }
    public string? BranchId { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}
