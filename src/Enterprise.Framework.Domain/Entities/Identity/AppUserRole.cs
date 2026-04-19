namespace Enterprise.Framework.Domain.Entities.Identity;

public class AppUserRole : BaseEntity {

public long UserId { get; set; }
long RoleId { get; set; }
AppUser User { get; set; } = null!;

    public AppRole Role { get; set; }
}



