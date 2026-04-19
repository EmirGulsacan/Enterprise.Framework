namespace Enterprise.Framework.Domain.Entities.Identity;

public class AppUserPermission : BaseEntity {

public long UserId { get; set; }
AppUser User { get; set; } = null!;

    public long PermissionId { get; set; }
AppPermission Permission { get; set; } = null!;

    public bool IsGranted { get; set; }
}



