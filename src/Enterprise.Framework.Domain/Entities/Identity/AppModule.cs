namespace Enterprise.Framework.Domain.Entities.Identity;

public class AppModule : BaseEntity {

public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
ICollection<AppPermission> Permissions { get; set; } = new List<AppPermission>();
}



