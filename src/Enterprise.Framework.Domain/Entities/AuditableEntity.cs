namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

public abstract class AuditableEntity : BaseEntity, IAuditableEntity, ISoftDeletable
{
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }
}
