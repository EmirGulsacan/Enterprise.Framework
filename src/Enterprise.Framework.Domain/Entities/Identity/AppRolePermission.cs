namespace Enterprise.Framework.Domain.Entities.Identity;

public class AppRolePermission : BaseEntity {

public long RoleId { get; set; }
long PermissionId { get; set; }
AppRole Role { get; set; } = null!;

    public AppPermission Permission { get; set; }
}



