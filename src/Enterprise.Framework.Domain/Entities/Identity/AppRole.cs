namespace Enterprise.Framework.Domain.Entities.Identity;

public class AppRole : BaseEntity {

public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();

    public ICollection<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();
}




