namespace Enterprise.Framework.Domain.Entities;

public class AppModule : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<AppPermission> Permissions { get; set; } = new List<AppPermission>();
}
