using Enterprise.Framework.Domain.Common;

namespace Enterprise.Framework.Domain.Entities;

public enum LocationType
{
    Region = 1,
    City = 2,
    Branch = 3,
    SubUnit = 4
}

public class Location : BaseEntity, IOrganizationBoundEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public LocationType Type { get; set; }
    public long? ParentId { get; set; }
    public string? OrganizationId { get; set; }
    public string? RegistryNumbers { get; set; }
    public Location? Parent { get; set; }
    public ICollection<Location> Children { get; set; } = new List<Location>();
}

