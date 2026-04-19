namespace Enterprise.Framework.Domain.Entities;

public class AppRolePermission : BaseEntity
{
    public long RoleId { get; set; }
    public long PermissionId { get; set; }
    public AppRole Role { get; set; } = null!;
    public AppPermission Permission { get; set; } = null!;
}
