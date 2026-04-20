namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

public class Labor : AuditableEntity
{
    public long MaintenanceId { get; set; }
    public Maintenance Maintenance { get; set; } = null!;

    public long EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
}

