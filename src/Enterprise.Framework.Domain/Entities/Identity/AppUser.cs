namespace Enterprise.Framework.Domain.Entities.Identity;

using Enterprise.Framework.Domain.Common;



public class AppUser : BaseEntity, IOrganizationBoundEntity {

public string IdentityId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? OrganizationId { get; set; }
string? BranchId { get; set; }
bool IsAdmin { get; set; }
bool IsActive { get; set; } = true;

    public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
}



