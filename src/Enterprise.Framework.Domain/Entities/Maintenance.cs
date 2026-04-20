namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;
using Enterprise.Framework.Domain.Common.Enums;

public class Maintenance : AuditableEntity
{
    public long AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;

    public ICollection<Labor> Labors { get; set; } = new List<Labor>();
}

