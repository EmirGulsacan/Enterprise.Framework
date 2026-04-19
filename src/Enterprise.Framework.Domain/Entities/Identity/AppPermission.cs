namespace Enterprise.Framework.Domain.Entities;

public class AppPermission : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long ModuleId { get; set; }
    public AppModule Module { get; set; } = null!;
}
