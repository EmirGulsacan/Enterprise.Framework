namespace Enterprise.Framework.Domain.Entities;

public class AppUserPermission : BaseEntity
{
    public long UserId { get; set; }
    public AppUser User { get; set; } = null!;
    public long PermissionId { get; set; }
    public AppPermission Permission { get; set; } = null!;
    public bool IsGranted { get; set; }
}
