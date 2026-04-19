namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

public class Maintenance : BaseEntity, IAuditableEntity
{
    public long AssetId { get; set; }
    public Asset Asset { get; set; } = null!;

    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }

    public ICollection<Labor> Labors { get; set; } = new List<Labor>();
}
