namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

public class Asset : BaseEntity, IAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public string Status { get; set; } = "Active";
    
    public long? AssignedEmployeeId { get; set; }
    public Employee? AssignedEmployee { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }

    public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
}
