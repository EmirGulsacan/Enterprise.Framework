namespace Enterprise.Framework.Domain.Entities.Identity;

public class AppPermission : BaseEntity {

public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
long ModuleId { get; set; }
AppModule Module { get; set; } = null!;

    public ICollection<AppRolePermission> RolePermissions { get; set; } = new List<AppRolePermission>();

    public ICollection<AppUserPermission> UserPermissions { get; set; } = new List<AppUserPermission>();
}




