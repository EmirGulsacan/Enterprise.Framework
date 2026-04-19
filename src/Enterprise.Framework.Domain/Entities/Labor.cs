namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

public class Labor : BaseEntity, IAuditableEntity
{
    public long MaintenanceId { get; set; }
    public Maintenance Maintenance { get; set; } = null!;

    public long EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
