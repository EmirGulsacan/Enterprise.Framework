namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;
using Enterprise.Framework.Domain.Common.Enums;

public class Asset : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public AssetStatus Status { get; set; } = AssetStatus.Active;

    public long? AssignedEmployeeId { get; set; }
    public Employee? AssignedEmployee { get; set; }

    public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
}

