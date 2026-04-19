namespace Enterprise.Framework.Domain.Entities;

public class AppUserRole : BaseEntity
{
    public long UserId { get; set; }
    public long RoleId { get; set; }
    public AppUser User { get; set; } = null!;
    public AppRole Role { get; set; } = null!;
}
